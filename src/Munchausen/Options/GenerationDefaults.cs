namespace Munchausen;

/// <summary>
/// Definition-level defaults. Each property is optional; a null property states
/// no opinion, so merging two instances applies per-property last-write-wins.
/// </summary>
public sealed record GenerationDefaults
{
    /// <summary>Default seed for reproducible generation.</summary>
    public int? Seed { get; init; }

    /// <summary>Default fixed reference time.</summary>
    public DateTimeOffset? ReferenceTime { get; init; }

    /// <summary>Maximum object-graph depth before the cycle behavior applies.</summary>
    public int? MaximumDepth { get; init; }

    /// <summary>Behavior when a reference cycle or the maximum depth is reached.</summary>
    public CycleBehavior? CycleBehavior { get; init; }

    /// <summary>Semantic inference mode.</summary>
    public SemanticInferenceMode? SemanticInference { get; init; }
}
