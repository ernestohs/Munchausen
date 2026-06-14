using System.Collections.Immutable;
using Munchausen.Inference;
using Munchausen.Metadata;

namespace Munchausen.Compilation;

/// <summary>
/// Base for a construction strategy. Concrete plans (compiled constructor invoker,
/// user <c>ConstructWith</c> delegate) are added by the constructor planner in M5.
/// </summary>
internal abstract class ConstructionPlan;

/// <summary>
/// The Explain-facing record for one member: where its value comes from, the
/// generator name, confidence, rejected candidates, and overridden rules.
/// </summary>
internal sealed record MemberReportData(
    InferenceSource Source,
    string GeneratorName,
    InferenceConfidence? Confidence,
    IReadOnlyList<RejectedCandidate> RejectedCandidates,
    IReadOnlyList<string> OverriddenRules);

/// <summary>A single member's compiled value source plus its report data.</summary>
internal sealed class MemberPlan(MemberMetadata member, MemberAccessor accessor, ValueSource source, MemberReportData report)
{
    public MemberMetadata Member { get; } = member;

    public MemberAccessor Accessor { get; } = accessor;

    public ValueSource Source { get; } = source;

    public MemberReportData Report { get; } = report;
}

/// <summary>A derivation that runs after population, in registration order.</summary>
internal sealed class DerivationPlan(
    MemberMetadata member,
    MemberAccessor accessor,
    Func<GenerationContext, object, object?> generator,
    int registrationIndex)
{
    public MemberMetadata Member { get; } = member;

    public MemberAccessor Accessor { get; } = accessor;

    public Func<GenerationContext, object, object?> Generator { get; } = generator;

    public int RegistrationIndex { get; } = registrationIndex;
}

/// <summary>The compiled plan for one model type.</summary>
internal sealed class TypePlan(
    Type modelType,
    ConstructionPlan construction,
    ImmutableArray<MemberPlan> members,
    ImmutableArray<DerivationPlan> derivations)
{
    public Type ModelType { get; } = modelType;

    public ConstructionPlan Construction { get; } = construction;

    public ImmutableArray<MemberPlan> Members { get; } = members;

    public ImmutableArray<DerivationPlan> Derivations { get; } = derivations;
}

/// <summary>
/// A frozen, immutable generation plan: the root type plan plus every reachable
/// child plan, shared by reference for recursive graphs.
/// (BuildDiagnostics is added with the diagnostics layer in M5.)
/// </summary>
internal sealed class GenerationPlan(
    TypePlan root,
    ImmutableDictionary<Type, TypePlan> reachablePlans,
    GenerationDefaults defaults,
    string? definitionName)
{
    public TypePlan Root { get; } = root;

    public ImmutableDictionary<Type, TypePlan> ReachablePlans { get; } = reachablePlans;

    public GenerationDefaults Defaults { get; } = defaults;

    public string? DefinitionName { get; } = definitionName;
}
