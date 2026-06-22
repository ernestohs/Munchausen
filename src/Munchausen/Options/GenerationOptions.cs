namespace Munchausen;

/// <summary>Per-call generation options. Overrides the definition's defaults for a single call.</summary>
public sealed record GenerationOptions
{
    /// <summary>Seed for this call; when null, the definition default (or entropy) is used.</summary>
    public int? Seed { get; init; }

    /// <summary>Fixed reference time for this call; when null, the definition default is used.</summary>
    public DateTimeOffset? ReferenceTime { get; init; }

    /// <summary>Time source for resolving the reference time; integrates with the standard testing seam.</summary>
    public TimeProvider? TimeProvider { get; init; }
}
