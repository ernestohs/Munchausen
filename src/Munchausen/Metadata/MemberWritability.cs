namespace Munchausen.Metadata;

/// <summary>How a member's value may be assigned.</summary>
internal enum MemberWritability
{
    /// <summary>Public setter; assignable any time after construction.</summary>
    Writable,

    /// <summary>Init-only setter; assignable through the accessor, never via plain C#.</summary>
    InitOnly,

    /// <summary>No public setter; preserved, never populated.</summary>
    ReadOnly,
}
