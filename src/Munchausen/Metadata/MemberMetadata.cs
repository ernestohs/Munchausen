using System.Reflection;

namespace Munchausen.Metadata;

/// <summary>
/// Immutable, reflection-derived description of a single populatable member.
/// </summary>
internal sealed class MemberMetadata
{
    public MemberMetadata(
        MemberInfo member,
        Type valueType,
        string name,
        MemberWritability writability,
        NullabilityKind nullability,
        bool isRequired,
        IReadOnlyList<Attribute> attributes,
        int declarationOrder)
    {
        Member = member;
        ValueType = valueType;
        Name = name;
        Writability = writability;
        Nullability = nullability;
        IsRequired = isRequired;
        Attributes = attributes;
        DeclarationOrder = declarationOrder;
    }

    public MemberInfo Member { get; }

    public Type ValueType { get; }

    public string Name { get; }

    public MemberWritability Writability { get; }

    public NullabilityKind Nullability { get; }

    /// <summary>True for the C# <c>required</c> modifier or DataAnnotations <c>[Required]</c>.</summary>
    public bool IsRequired { get; }

    public IReadOnlyList<Attribute> Attributes { get; }

    /// <summary>Zero-based position in <see cref="MemberInfo.MetadataToken"/> order.</summary>
    public int DeclarationOrder { get; }
}
