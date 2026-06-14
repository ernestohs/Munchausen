namespace Munchausen.Tests.Fixtures;

/// <summary>A required member of an unsupported type — triggers LIE003.</summary>
public sealed class RequiredUnsupportedModel
{
    public required IComparable Thing { get; set; }
}

/// <summary>One of several constructors is marked — attribute selection.</summary>
public sealed class AttributeCtorModel
{
    public AttributeCtorModel()
    {
    }

    [LieConstructor]
    public AttributeCtorModel(int seed) => Seed = seed;

    public AttributeCtorModel(string label) => Label = label;

    public int Seed { get; set; }

    public string? Label { get; set; }
}

/// <summary>Parameterless vs a richer constructor — most-resolvable wins.</summary>
public sealed class MostResolvableModel
{
    public MostResolvableModel()
    {
    }

    public MostResolvableModel(string label, int count)
    {
        Label = label;
        Count = count;
    }

    public string? Label { get; set; }

    public int Count { get; set; }
}

/// <summary>Parameterless ties with an unresolvable constructor — prefer parameterless.</summary>
public sealed class ParameterlessTieModel
{
    public ParameterlessTieModel()
    {
    }

    public ParameterlessTieModel(IComparable unresolvable) => _unused = unresolvable;

    private readonly IComparable? _unused;
}

/// <summary>Two equally resolvable constructors, no parameterless — ambiguous (LIE004).</summary>
public sealed class AmbiguousCtorModel
{
    public AmbiguousCtorModel(string a) => A = a;

    public AmbiguousCtorModel(int b) => B = b;

    public string? A { get; }

    public int B { get; }
}
