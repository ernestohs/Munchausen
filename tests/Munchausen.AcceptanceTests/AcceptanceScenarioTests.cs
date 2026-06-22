using Munchausen;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.AcceptanceTests;

/// <summary>The v1.0-tagged end-to-end scenarios from API_DESIGN.md, as compiled tests.</summary>
public sealed class AcceptanceScenarioTests
{
    [Fact]
    public void AutomaticInference_GeneratesWithoutConfiguration()
    {
        IReadOnlyList<Car> cars = Lie<Car>.Generate(100);

        Assert.Equal(100, cars.Count);
        Assert.All(cars, car =>
        {
            Assert.NotEmpty(car.Make);
            Assert.NotNull(car.Owner);
            Assert.Contains("@", car.Owner.Email);
        });
    }

    [Fact]
    public void ReproducibleDefinedGeneration_IsEquivalentAndExplainable()
    {
        var recentCars = Lie.Define<Car>()
            .WithName("Recent cars")
            .With(car => car.Year, data => data.Random.Int(2020, 2026))
            .WithSeed(42)
            .Build();

        IReadOnlyList<Car> first = recentCars.Generate(100);
        IReadOnlyList<Car> second = recentCars.Generate(100);

        Assert.Equal(Project(first), Project(second));
        Assert.All(first, car => Assert.InRange(car.Year, 2020, 2026));

        InferenceReport report = recentCars.Explain();
        string text = report.ToText();

        Assert.Equal("Recent cars", report.DefinitionName);
        Assert.NotEmpty(text);
        Assert.Contains("Car.Make", text);
    }

    private static IEnumerable<string> Project(IEnumerable<Car> cars) =>
        cars.Select(car =>
            $"{car.Id}|{car.Make}|{car.Model}|{car.Year}|{car.Price}|" +
            $"{car.Owner.Id}|{car.Owner.FirstName}|{car.Owner.LastName}|{car.Owner.Email}");
}
