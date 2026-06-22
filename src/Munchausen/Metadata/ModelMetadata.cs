namespace Munchausen.Metadata;

/// <summary>
/// Immutable, cached description of a model type: its public constructors and its
/// populatable members in deterministic order.
/// </summary>
internal sealed class ModelMetadata
{
    public ModelMetadata(
        Type type,
        IReadOnlyList<ConstructorMetadata> constructors,
        IReadOnlyList<MemberMetadata> members,
        bool isNullableAnnotationContextEnabled)
    {
        Type = type;
        Constructors = constructors;
        Members = members;
        IsNullableAnnotationContextEnabled = isNullableAnnotationContextEnabled;
    }

    public Type Type { get; }

    public IReadOnlyList<ConstructorMetadata> Constructors { get; }

    /// <summary>Members in <see cref="System.Reflection.MemberInfo.MetadataToken"/> order.</summary>
    public IReadOnlyList<MemberMetadata> Members { get; }

    public bool IsNullableAnnotationContextEnabled { get; }
}
