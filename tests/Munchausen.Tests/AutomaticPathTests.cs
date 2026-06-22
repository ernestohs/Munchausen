using Munchausen;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class AutomaticPathTests
{
    [Fact]
    public void AutomaticPath_EqualsInferredOnlyDefinition_ForSameSeed()
    {
        IReadOnlyList<Car> automatic = Lie<Car>.Generate(5, new GenerationOptions { Seed = 42 });
        IReadOnlyList<Car> defined = Lie.Define<Car>().Build().Generate(5, new GenerationOptions { Seed = 42 });

        Assert.Equal(Project(automatic), Project(defined));
    }

    [Fact]
    public void AutomaticPath_IsCachedAndReproducible()
    {
        Car first = Lie<Car>.Generate(new GenerationOptions { Seed = 7 });
        Car second = Lie<Car>.Generate(new GenerationOptions { Seed = 7 });

        Assert.Equal($"{first.Id}|{first.Make}", $"{second.Id}|{second.Make}");
    }

    [Fact]
    public void AutomaticPath_CachesCompilationFailure()
    {
        // RequiredUnsupportedModel fails to compile (LIE003); the failure is cached.
        var firstException = Assert.Throws<LieDefinitionException>(() => Lie<RequiredUnsupportedModel>.Generate());
        var secondException = Assert.Throws<LieDefinitionException>(() => Lie<RequiredUnsupportedModel>.Generate());

        Assert.Same(firstException, secondException);
    }

    [Fact]
    public void Context_GenerateChild_WorksInConstructWith()
    {
        var definition = Lie.Define<Car>()
            .ConstructWith(data => new Car { Owner = data.Generate<Owner>() })
            .Build();

        Car car = definition.Generate(new GenerationOptions { Seed = 3 });

        Assert.NotNull(car.Owner);
        Assert.NotEmpty(car.Owner.FirstName);
    }

    [Fact]
    public void Generation_HasNoPerObjectReflectionAllocation()
    {
        var definition = Lie.Define<Primitives>().Build();
        _ = definition.Generate(new GenerationOptions { Seed = 1 }); // warm up JIT and caches

        const int count = 50_000;
        long before = GC.GetAllocatedBytesForCurrentThread();
        _ = definition.Generate(count, new GenerationOptions { Seed = 1 });
        long after = GC.GetAllocatedBytesForCurrentThread();

        double perObject = (after - before) / (double)count;
        // Boxing of value-type members is accepted in v1.0 (~400 bytes for this 5-member
        // model); per-object reflection (Activator/Invoke/GetValue) would push this far higher.
        Assert.True(perObject < 600, $"per-object allocation was {perObject:F1} bytes");
    }

    private static IEnumerable<string> Project(IEnumerable<Car> cars) =>
        cars.Select(car => $"{car.Id}|{car.Make}|{car.Model}|{car.Year}|{car.Price}|{car.Owner.FirstName}");
}
