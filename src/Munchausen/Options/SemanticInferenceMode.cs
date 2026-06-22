namespace Munchausen;

/// <summary>How aggressively semantic (name-based) inference accepts candidate matches.</summary>
public enum SemanticInferenceMode
{
    /// <summary>Accept high-confidence semantic matches only.</summary>
    Conservative,

    /// <summary>Accept high- and medium-confidence matches. This is the default.</summary>
    Balanced,

    /// <summary>Also accept low-confidence matches.</summary>
    Aggressive,

    /// <summary>Disable semantic inference entirely.</summary>
    Disabled,
}
