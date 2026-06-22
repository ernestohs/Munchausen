namespace Munchausen;

/// <summary>Where a member's value source came from.</summary>
public enum InferenceSource
{
    /// <summary>An explicit <c>With</c> rule.</summary>
    Explicit,

    /// <summary>A <c>Derive</c> derivation.</summary>
    Derived,

    /// <summary>Preserved (read-only or <c>Preserve</c>).</summary>
    Preserved,

    /// <summary>Ignored via <c>Ignore</c>.</summary>
    Ignored,

    /// <summary>A supported attribute (v1.1).</summary>
    Attribute,

    /// <summary>A custom inference provider (v1.1).</summary>
    CustomProvider,

    /// <summary>Semantic (name-and-model) inference.</summary>
    Semantic,

    /// <summary>Type-based inference.</summary>
    Type,

    /// <summary>A nested or child definition.</summary>
    ChildDefinition,

    /// <summary>Unsupported; left untouched.</summary>
    Unsupported,
}
