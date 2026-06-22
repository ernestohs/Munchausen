using Munchausen.Metadata;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class MemberAccessorTests
{
    // false = compiled expressions, true = reflection fallback. Both must behave identically.
    private static IMemberAccessorFactory Factory(bool useReflectionFallback) =>
        new CompiledExpressionAccessorFactory(useReflectionFallback);

    private static MemberMetadata Member<T>(string name) =>
        new ReflectionModelMetadataProvider().GetMetadata(typeof(T)).Members.Single(m => m.Name == name);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WritableProperty_RoundTrips(bool useReflectionFallback)
    {
        IMemberAccessorFactory factory = Factory(useReflectionFallback);
        MemberAccessor accessor = factory.CreateAccessor(Member<Owner>("FirstName"));
        var owner = new Owner();

        Assert.NotNull(accessor.Setter);
        Assert.NotNull(accessor.Getter);

        accessor.Setter!(owner, "Ada");

        Assert.Equal("Ada", owner.FirstName);
        Assert.Equal("Ada", accessor.Getter!(owner));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InitOnlyProperty_CanBeSetThroughAccessor(bool useReflectionFallback)
    {
        MemberAccessor accessor = Factory(useReflectionFallback).CreateAccessor(Member<WritabilityFixture>("InitOnly"));
        var instance = new WritabilityFixture();

        Assert.NotNull(accessor.Setter);
        accessor.Setter!(instance, "set-after-construction");

        Assert.Equal("set-after-construction", instance.InitOnly);
        Assert.Equal("set-after-construction", accessor.Getter!(instance));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadOnlyProperty_HasNoSetterButReadsValue(bool useReflectionFallback)
    {
        MemberAccessor accessor = Factory(useReflectionFallback).CreateAccessor(Member<WritabilityFixture>("Computed"));

        Assert.Null(accessor.Setter);
        Assert.NotNull(accessor.Getter);
        Assert.Equal(42, accessor.Getter!(new WritabilityFixture()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ValueTypeMember_BoxesAndUnboxes(bool useReflectionFallback)
    {
        MemberAccessor accessor = Factory(useReflectionFallback).CreateAccessor(Member<WritabilityFixture>("Writable"));
        var instance = new WritabilityFixture();

        accessor.Setter!(instance, 99);

        Assert.Equal(99, instance.Writable);
        Assert.Equal(99, accessor.Getter!(instance));
    }
}
