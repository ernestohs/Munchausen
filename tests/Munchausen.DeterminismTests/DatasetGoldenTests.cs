using Munchausen;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.DeterminismTests;

/// <summary>
/// Dataset-backed golden for a real test model: pins the seeded output of the
/// semantic and dataset generators (names, email) plus traversal order across
/// process runs. A failure is a repeatability-contract event.
/// </summary>
public sealed class DatasetGoldenTests
{
    [Fact]
    public void Owner_Seed2026_ReproducesCapturedOutput()
    {
        string[] expected = File.ReadAllLines(
            Path.Combine(AppContext.BaseDirectory, "Goldens", "generation-owner-seed2026.txt"))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        Owner owner = Lie.Define<Owner>().Build().Generate(new GenerationOptions { Seed = 2026 });

        Assert.Equal(expected[0], owner.Id.ToString("D"));
        Assert.Equal(expected[1], owner.FirstName);
        Assert.Equal(expected[2], owner.LastName);
        Assert.Equal(expected[3], owner.Email);
    }
}
