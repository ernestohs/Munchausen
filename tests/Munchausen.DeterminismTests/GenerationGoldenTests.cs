using System.Globalization;
using Munchausen;
using Xunit;

namespace Munchausen.DeterminismTests;

/// <summary>
/// First runtime golden: pins the seeded output of the pure-PRNG type generators
/// and the traversal order across process runs. Dataset-backed members (strings,
/// dates, semantic generators) are deferred to M7, so the golden model uses only
/// type-default members. A failure is a repeatability-contract event.
/// </summary>
public sealed class GenerationGoldenTests
{
    public sealed class GoldenModel
    {
        public int Alpha { get; set; }
        public bool Beta { get; set; }
        public Guid Gamma { get; set; }
        public long Delta { get; set; }
        public double Score { get; set; }
    }

    [Fact]
    public void GoldenModel_Seed2024_ReproducesCapturedOutput()
    {
        string[] expected = LoadGolden("generation-goldenmodel-seed2024.txt");

        GoldenModel model = Lie.Define<GoldenModel>().Build()
            .Generate(new GenerationOptions { Seed = 2024 });

        Assert.Equal(expected[0], model.Alpha.ToString(CultureInfo.InvariantCulture));
        Assert.Equal(expected[1], model.Beta ? "true" : "false");
        Assert.Equal(expected[2], model.Gamma.ToString("D"));
        Assert.Equal(expected[3], model.Delta.ToString(CultureInfo.InvariantCulture));
        Assert.Equal(expected[4], model.Score.ToString("R", CultureInfo.InvariantCulture));
    }

    private static string[] LoadGolden(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Goldens", fileName);
        return File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }
}
