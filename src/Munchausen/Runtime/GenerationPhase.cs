namespace Munchausen;

/// <summary>The generation lifecycle phase during which a failure occurred.</summary>
public enum GenerationPhase
{
    /// <summary>Object construction (a constructor or <c>ConstructWith</c> delegate).</summary>
    Construction,

    /// <summary>The before-generate hook (v1.1).</summary>
    BeforeGenerateHook,

    /// <summary>Member population (a <c>With</c> generator or inferred source).</summary>
    MemberPopulation,

    /// <summary>A <c>Derive</c> derivation.</summary>
    Derivation,

    /// <summary>The after-generate hook (v1.1).</summary>
    AfterGenerateHook,

    /// <summary>Validation (v1.1).</summary>
    Validation,

    /// <summary>Uniqueness commit (v1.1).</summary>
    UniquenessCommit,
}
