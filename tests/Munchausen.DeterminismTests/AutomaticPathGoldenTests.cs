using System.Globalization;
using Munchausen;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.DeterminismTests;

/// <summary>
/// Final golden: the canonical Car through the cached automatic path, with a fixed
/// seed and reference time so Vehicle.Year is deterministic. Pins the whole
/// inference + dataset + traversal pipeline across process runs.
/// </summary>
public sealed class AutomaticPathGoldenTests
{
    [Fact]
    public void AutomaticCar_Seed2026_ReproducesCapturedOutput()
    {
        string[] expected = File.ReadAllLines(
            Path.Combine(AppContext.BaseDirectory, "Goldens", "automatic-car-seed2026.txt"))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        var options = new GenerationOptions
        {
            Seed = 2026,
            ReferenceTime = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };
        Car car = Lie<Car>.Generate(options);

        CultureInfo invariant = CultureInfo.InvariantCulture;
        Assert.Equal(expected[0], car.Id.ToString("D"));
        Assert.Equal(expected[1], car.Make);
        Assert.Equal(expected[2], car.Model);
        Assert.Equal(expected[3], car.Year.ToString(invariant));
        Assert.Equal(expected[4], car.Price.ToString(invariant));
        Assert.Equal(expected[5], car.Owner.FirstName);
        Assert.Equal(expected[6], car.Owner.LastName);
        Assert.Equal(expected[7], car.Owner.Email);
    }
}
