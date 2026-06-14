using Munchausen;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.AcceptanceTests;

/// <summary>
/// Exercises the v1.0 public surface from an external assembly (no InternalsVisibleTo),
/// proving the builder fluent chain and option types compile and are reachable.
/// Build()/Generate() throw NotImplementedException until later milestones.
/// </summary>
public sealed class BuilderSurfaceTests
{
    [Fact]
    public void FluentChain_Compiles_AndReturnsBuilder()
    {
        LieDefinitionBuilder<Car> builder = Lie.Define<Car>()
            .WithName("cars")
            .With(car => car.Make, "Saab")
            .With(car => car.Model, context => "900")
            .Derive(car => car.Year, (context, car) => 1989)
            .Ignore(car => car.Price)
            .Preserve(car => car.Owner)
            .ConstructWith(context => new Car())
            .WithSeed(42)
            .WithDefaults(new GenerationDefaults { MaximumDepth = 5 });

        Assert.NotNull(builder);
    }

    [Fact]
    public void CollectionSize_FactoriesProduceInclusiveRanges()
    {
        CollectionSize exact = CollectionSize.Exactly(3);
        Assert.Equal(3, exact.MinCount);
        Assert.Equal(3, exact.MaxCount);

        CollectionSize between = CollectionSize.Between(1, 5);
        Assert.Equal(1, between.MinCount);
        Assert.Equal(5, between.MaxCount);
    }
}
