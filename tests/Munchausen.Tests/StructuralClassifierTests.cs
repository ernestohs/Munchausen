using Munchausen.Inference;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.Tests;

public sealed class StructuralClassifierTests
{
    private static StructuralClassification Classify(Type type) => StructuralClassifier.Classify(type);

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(Uri))]
    [InlineData(typeof(int?))]
    [InlineData(typeof(DayOfWeek))] // enum
    public void Scalars_AreScalar(Type type)
    {
        Assert.Equal("Scalar", Classify(type).Kind.ToString());
    }

    [Fact]
    public void ByteArray_IsScalarNotCollection()
    {
        Assert.Equal("Scalar", Classify(typeof(byte[])).Kind.ToString());
    }

    [Theory]
    [InlineData(typeof(int[]), "Array")]
    [InlineData(typeof(List<int>), "List")]
    [InlineData(typeof(IList<int>), "IList")]
    [InlineData(typeof(ICollection<int>), "ICollection")]
    [InlineData(typeof(IEnumerable<int>), "IEnumerable")]
    [InlineData(typeof(IReadOnlyList<int>), "IReadOnlyList")]
    [InlineData(typeof(IReadOnlyCollection<int>), "IReadOnlyCollection")]
    [InlineData(typeof(Dictionary<string, int>), "Dictionary")]
    [InlineData(typeof(IDictionary<string, int>), "IDictionary")]
    [InlineData(typeof(IReadOnlyDictionary<string, int>), "IReadOnlyDictionary")]
    public void CollectionShapes_AreRecognized(Type type, string expectedShape)
    {
        StructuralClassification classification = Classify(type);
        Assert.Equal("Collection", classification.Kind.ToString());
        Assert.Equal(expectedShape, classification.Shape.ToString());
    }

    [Fact]
    public void Sequence_ElementType_IsCaptured()
    {
        Assert.Equal(typeof(int), Classify(typeof(List<int>)).ElementType);
        Assert.Equal(typeof(Order), Classify(typeof(List<Order>)).ElementType);
    }

    [Fact]
    public void Dictionary_KeyAndValueTypes_AreCaptured()
    {
        StructuralClassification classification = Classify(typeof(Dictionary<string, int>));
        Assert.Equal(typeof(string), classification.KeyType);
        Assert.Equal(typeof(int), classification.ValueType);
    }

    [Fact]
    public void ConcreteClass_IsNested()
    {
        Assert.Equal("Nested", Classify(typeof(Owner)).Kind.ToString());
        Assert.Equal("Nested", Classify(typeof(Order)).Kind.ToString());
    }

    [Theory]
    [InlineData(typeof(IComparable))]      // interface
    [InlineData(typeof(nint))]
    [InlineData(typeof(nuint))]
    [InlineData(typeof(Half))]
    [InlineData(typeof(Int128))]
    [InlineData(typeof(UInt128))]
    public void UnsupportedTypes_AreUnsupported(Type type)
    {
        Assert.Equal("Unsupported", Classify(type).Kind.ToString());
    }
}
