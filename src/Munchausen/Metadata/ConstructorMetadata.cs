using System.Reflection;

namespace Munchausen.Metadata;

/// <summary>
/// Immutable description of a public constructor and its parameters. Constructor
/// <em>selection</em> is the compiler's job (M5); this only captures the shape.
/// </summary>
internal sealed class ConstructorMetadata
{
    public ConstructorMetadata(ConstructorInfo constructor, IReadOnlyList<ParameterMetadata> parameters)
    {
        Constructor = constructor;
        Parameters = parameters;
    }

    public ConstructorInfo Constructor { get; }

    public IReadOnlyList<ParameterMetadata> Parameters { get; }
}

/// <summary>Immutable description of a single constructor parameter.</summary>
internal sealed class ParameterMetadata
{
    public ParameterMetadata(
        ParameterInfo parameter,
        Type valueType,
        string name,
        NullabilityKind nullability,
        IReadOnlyList<Attribute> attributes)
    {
        Parameter = parameter;
        ValueType = valueType;
        Name = name;
        Nullability = nullability;
        Attributes = attributes;
    }

    public ParameterInfo Parameter { get; }

    public Type ValueType { get; }

    public string Name { get; }

    public NullabilityKind Nullability { get; }

    public IReadOnlyList<Attribute> Attributes { get; }
}
