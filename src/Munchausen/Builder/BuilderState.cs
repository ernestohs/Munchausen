using System.Linq.Expressions;

namespace Munchausen.Builder;

/// <summary>The kind of rule a <see cref="MemberRuleRecord"/> represents.</summary>
internal enum MemberRuleKind
{
    WithValue,
    WithGenerator,
    Derive,
    Ignore,
    Preserve,
}

/// <summary>
/// A single captured member rule. Nothing is parsed or validated until Build();
/// this preserves registration order, which Derive and last-write-wins require.
/// </summary>
internal sealed record MemberRuleRecord(
    LambdaExpression MemberExpression,
    MemberRuleKind Kind,
    object? Payload,
    int RegistrationIndex);

/// <summary>A captured <c>ConstructWith</c> delegate.</summary>
internal sealed record ConstructionRecord(object Constructor);

/// <summary>
/// Mutable, not-thread-safe state accumulated by <see cref="LieDefinitionBuilder{T}"/>.
/// v1.1 attaches further ordered lists (providers, hooks, validators, ...).
/// </summary>
internal sealed class BuilderState
{
    public string? Name { get; set; }

    public List<MemberRuleRecord> MemberRules { get; } = new();

    public ConstructionRecord? Construction { get; set; }

    public GenerationDefaults Defaults { get; set; } = new();

    public void AddMemberRule(LambdaExpression expression, MemberRuleKind kind, object? payload)
        => MemberRules.Add(new MemberRuleRecord(expression, kind, payload, MemberRules.Count));

    /// <summary>Merges defaults with per-property last-write-wins (null = no opinion).</summary>
    public void MergeDefaults(GenerationDefaults incoming)
    {
        Defaults = Defaults with
        {
            Seed = incoming.Seed ?? Defaults.Seed,
            ReferenceTime = incoming.ReferenceTime ?? Defaults.ReferenceTime,
            MaximumDepth = incoming.MaximumDepth ?? Defaults.MaximumDepth,
            CycleBehavior = incoming.CycleBehavior ?? Defaults.CycleBehavior,
            SemanticInference = incoming.SemanticInference ?? Defaults.SemanticInference,
        };
    }
}
