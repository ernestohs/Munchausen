using System.Collections.Immutable;
using Munchausen.Builder;
using Munchausen.Diagnostics;
using Munchausen.Inference;
using Munchausen.Metadata;

namespace Munchausen.Compilation;

/// <summary>
/// Compiles a snapshot of <see cref="BuilderState"/> into an immutable
/// <see cref="GenerationPlan"/>: resolve expressions, detect conflicts, plan
/// construction, infer per member, validate, compile reachable child plans
/// (cycle-safe via a visited worklist), and freeze. Every error is collected; a
/// non-empty error set throws <see cref="LieDefinitionException"/>.
/// </summary>
internal sealed class DefinitionCompiler
{
    private readonly IModelMetadataProvider _metadataProvider;
    private readonly IMemberAccessorFactory _accessorFactory;
    private readonly InferenceEngine _engine = new();
    private readonly ConstructorPlanner _planner;
    private readonly ExpressionMemberResolver _resolver = new();

    public DefinitionCompiler(IModelMetadataProvider metadataProvider, IMemberAccessorFactory accessorFactory)
    {
        _metadataProvider = metadataProvider;
        _accessorFactory = accessorFactory;
        _planner = new ConstructorPlanner(_engine);
    }

    public static DefinitionCompiler Default { get; } =
        new(ReflectionModelMetadataProvider.Shared, new CompiledExpressionAccessorFactory());

    public GenerationPlan Compile(Type rootType, BuilderState state)
    {
        var diagnostics = new DiagnosticBag(rootType);

        if (state.Name is not null && string.IsNullOrWhiteSpace(state.Name))
        {
            diagnostics.InvalidOption("Definition name must not be empty or whitespace.");
        }

        SemanticInferenceMode mode = state.Defaults.SemanticInference ?? SemanticInferenceMode.Balanced;
        ResolvedRules rootRules = ResolveRules(state, diagnostics);

        var plans = new Dictionary<Type, TypePlan>();
        var worklist = new Queue<Type>();
        worklist.Enqueue(rootType);

        while (worklist.Count > 0)
        {
            Type type = worklist.Dequeue();
            if (plans.ContainsKey(type))
            {
                continue;
            }

            bool isRoot = type == rootType;
            plans[type] = BuildTypePlan(
                type,
                isRoot ? rootRules : ResolvedRules.Empty,
                isRoot ? state.Construction?.Constructor : null,
                mode,
                diagnostics,
                worklist);
        }

        if (diagnostics.HasErrors)
        {
            throw LieDefinitionException.FromDiagnostics(rootType, diagnostics.Diagnostics);
        }

        ImmutableArray<LieDiagnostic> buildDiagnostics = diagnostics.Diagnostics
            .Where(d => d.Severity != LieDiagnosticSeverity.Error)
            .ToImmutableArray();

        return new GenerationPlan(
            plans[rootType], plans.ToImmutableDictionary(), state.Defaults, state.Name, buildDiagnostics);
    }

    private TypePlan BuildTypePlan(
        Type type,
        ResolvedRules rules,
        object? constructWith,
        SemanticInferenceMode mode,
        DiagnosticBag diagnostics,
        Queue<Type> worklist)
    {
        ModelMetadata metadata = _metadataProvider.GetMetadata(type);
        var sources = new MemberSourceBuilder(_engine, diagnostics, type, worklist.Enqueue);
        ConstructionPlan construction = _planner.Plan(type, metadata, constructWith, mode, sources, diagnostics);

        var members = ImmutableArray.CreateBuilder<MemberPlan>();
        foreach (MemberMetadata member in metadata.Members)
        {
            int token = member.Member.MetadataToken;
            if (rules.Derives.ContainsKey(token))
            {
                continue; // Derived members are set in the derivation phase, not populated.
            }

            MemberAccessor accessor = _accessorFactory.CreateAccessor(member);
            (ValueSource source, MemberReportData report) = rules.Effective.TryGetValue(token, out EffectiveRule? rule)
                ? BuildExplicitSource(rule)
                : BuildInferredSource(type, member, mode, sources);

            members.Add(new MemberPlan(member, accessor, source, report));
        }

        var derivations = ImmutableArray.CreateBuilder<DerivationPlan>();
        foreach ((int token, object payload, int registrationIndex) in rules.Derives.Values.OrderBy(d => d.RegistrationIndex))
        {
            MemberMetadata? member = metadata.Members.FirstOrDefault(m => m.Member.MetadataToken == token);
            if (member is null)
            {
                continue;
            }

            derivations.Add(new DerivationPlan(
                member,
                _accessorFactory.CreateAccessor(member),
                UserDelegateAdapter.Derive((Delegate)payload, type),
                registrationIndex));
        }

        return new TypePlan(type, construction, members.ToImmutable(), derivations.ToImmutable());
    }

    private (ValueSource Source, MemberReportData Report) BuildInferredSource(
        Type type, MemberMetadata member, SemanticInferenceMode mode, MemberSourceBuilder sources)
    {
        if (member.Writability == MemberWritability.ReadOnly)
        {
            return (
                new SkippedSource(SkipReason.Preserved),
                new MemberReportData(InferenceSource.Preserved, "Preserve (read-only)", null, [], []));
        }

        var context = new MemberInferenceContext(type, member, member.Name, mode);
        ValueSource source = sources.BuildInferred(context, member.IsRequired, out ResolvedSource resolved);
        var report = new MemberReportData(
            resolved.Source,
            resolved.GeneratorName,
            resolved.Confidence,
            resolved.RejectedCandidates ?? Array.Empty<RejectedCandidate>(),
            Array.Empty<string>());
        return (source, report);
    }

    private static (ValueSource Source, MemberReportData Report) BuildExplicitSource(EffectiveRule rule)
    {
        switch (rule.Kind)
        {
            case MemberRuleKind.WithValue:
                return (
                    new ConstantSource(rule.Payload),
                    new MemberReportData(InferenceSource.Explicit, "With(value)", null, [], []));

            case MemberRuleKind.WithGenerator:
                return (
                    new DelegateSource(
                        UserDelegateAdapter.Value((Delegate)rule.Payload!), "With(generator)", isUserDelegate: true),
                    new MemberReportData(InferenceSource.Explicit, "With(generator)", null, [], []));

            case MemberRuleKind.Ignore:
                return (
                    new SkippedSource(SkipReason.Ignored),
                    new MemberReportData(InferenceSource.Ignored, "Ignore", null, [], []));

            default: // Preserve
                return (
                    new SkippedSource(SkipReason.Preserved),
                    new MemberReportData(InferenceSource.Preserved, "Preserve", null, [], []));
        }
    }

    private ResolvedRules ResolveRules(BuilderState state, DiagnosticBag diagnostics)
    {
        var grouped = new Dictionary<int, List<(MemberRuleKind Kind, object? Payload, int Index)>>();
        var names = new Dictionary<int, string>();

        foreach (MemberRuleRecord record in state.MemberRules)
        {
            ExpressionResolution resolution = _resolver.Resolve(record.MemberExpression);
            if (!resolution.IsResolved)
            {
                diagnostics.InvalidMemberExpression(resolution.Message!);
                continue;
            }

            int token = resolution.Member!.MetadataToken;
            names[token] = resolution.Member.Name;
            if (!grouped.TryGetValue(token, out var list))
            {
                grouped[token] = list = new List<(MemberRuleKind, object?, int)>();
            }

            list.Add((record.Kind, record.Payload, record.RegistrationIndex));
        }

        var effective = new Dictionary<int, EffectiveRule>();
        var derives = new Dictionary<int, (int Token, object Payload, int RegistrationIndex)>();

        foreach ((int token, var rules) in grouped)
        {
            var withs = rules.Where(r => r.Kind is MemberRuleKind.WithValue or MemberRuleKind.WithGenerator).ToList();
            var deriveRules = rules.Where(r => r.Kind == MemberRuleKind.Derive).ToList();
            bool hasIgnore = rules.Exists(r => r.Kind == MemberRuleKind.Ignore);
            bool hasPreserve = rules.Exists(r => r.Kind == MemberRuleKind.Preserve);

            bool conflict =
                deriveRules.Count > 1
                || (deriveRules.Count >= 1 && (withs.Count > 0 || hasIgnore || hasPreserve))
                || (withs.Count > 0 && (hasIgnore || hasPreserve))
                || (hasIgnore && hasPreserve);

            if (conflict)
            {
                diagnostics.ConflictingRules(names[token]);
                continue;
            }

            if (deriveRules.Count == 1)
            {
                derives[token] = (token, deriveRules[0].Payload!, deriveRules[0].Index);
            }
            else if (withs.Count > 0)
            {
                effective[token] = new EffectiveRule(withs[^1].Kind, withs[^1].Payload);
            }
            else if (hasIgnore)
            {
                effective[token] = new EffectiveRule(MemberRuleKind.Ignore, null);
            }
            else if (hasPreserve)
            {
                effective[token] = new EffectiveRule(MemberRuleKind.Preserve, null);
            }
        }

        return new ResolvedRules(effective, derives);
    }

    private sealed record EffectiveRule(MemberRuleKind Kind, object? Payload);

    private sealed record ResolvedRules(
        Dictionary<int, EffectiveRule> Effective,
        Dictionary<int, (int Token, object Payload, int RegistrationIndex)> Derives)
    {
        public static ResolvedRules Empty { get; } = new(new(), new());
    }
}
