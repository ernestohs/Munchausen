using Munchausen;
using Munchausen.Compilation;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class ConstructorSelectionTests
{
    private static CompiledConstructorPlan Construction<T>() =>
        Assert.IsType<CompiledConstructorPlan>(Lie.Define<T>().Build().Plan.Root.Construction);

    [Fact]
    public void Attribute_SelectsTheMarkedConstructor()
    {
        CompiledConstructorPlan plan = Construction<AttributeCtorModel>();
        ParameterPlan parameter = Assert.Single(plan.Parameters);
        Assert.Equal(typeof(int), parameter.ParameterType);
    }

    [Fact]
    public void MostResolvable_SelectsTheRichestConstructor()
    {
        Assert.Equal(2, Construction<MostResolvableModel>().Parameters.Length);
    }

    [Fact]
    public void ParameterlessTie_PrefersTheParameterlessConstructor()
    {
        Assert.Empty(Construction<ParameterlessTieModel>().Parameters);
    }

    [Fact]
    public void Ambiguous_FailsWithLIE004()
    {
        var exception = Assert.Throws<LieDefinitionException>(() => Lie.Define<AmbiguousCtorModel>().Build());
        Assert.Contains(exception.Diagnostics, d => d.Code == "LIE004");
    }

    [Fact]
    public void RecordPositional_SelectsThePrimaryConstructor()
    {
        Assert.Equal(4, Construction<CarRecord>().Parameters.Length);
    }
}
