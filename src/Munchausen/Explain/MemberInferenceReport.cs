namespace Munchausen;

/// <summary>How one member is resolved, for <see cref="InferenceReport"/>.</summary>
public sealed class MemberInferenceReport
{
    internal MemberInferenceReport(
        string memberPath,
        InferenceSource source,
        string generator,
        InferenceConfidence? confidence,
        string? originDefinition,
        int? derivationOrder,
        IReadOnlyList<string> overriddenRules)
    {
        MemberPath = memberPath;
        Source = source;
        Generator = generator;
        Confidence = confidence;
        OriginDefinition = originDefinition;
        DerivationOrder = derivationOrder;
        OverriddenRules = overriddenRules;
    }

    /// <summary>The member path, e.g. <c>Email</c>.</summary>
    public string MemberPath { get; }

    /// <summary>Where the value source came from.</summary>
    public InferenceSource Source { get; }

    /// <summary>The generator name, e.g. <c>Internet.Email</c>.</summary>
    public string Generator { get; }

    /// <summary>Match confidence, when the source is semantic.</summary>
    public InferenceConfidence? Confidence { get; }

    /// <summary>The originating named definition, when included or from a child definition.</summary>
    public string? OriginDefinition { get; }

    /// <summary>The registration order, when the member is derived.</summary>
    public int? DerivationOrder { get; }

    /// <summary>Rules overridden by a later rule on the same member.</summary>
    public IReadOnlyList<string> OverriddenRules { get; }
}
