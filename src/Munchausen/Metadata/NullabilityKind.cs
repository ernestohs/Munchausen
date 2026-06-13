namespace Munchausen.Metadata;

/// <summary>The nullability of a member's declared type.</summary>
internal enum NullabilityKind
{
    /// <summary>Non-nullable reference type, or a non-<see cref="Nullable{T}"/> value type.</summary>
    NonNullable,

    /// <summary>Nullable reference type, or <see cref="Nullable{T}"/>.</summary>
    Nullable,

    /// <summary>Reference type compiled without nullable annotations (pre-NRT / oblivious).</summary>
    Oblivious,
}
