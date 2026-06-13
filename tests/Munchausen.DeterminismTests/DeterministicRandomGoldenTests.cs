using Munchausen.Runtime;
using Xunit;

namespace Munchausen.DeterminismTests;

/// <summary>
/// Pins the seeded byte stream of <see cref="DeterministicRandom"/> against a
/// captured golden. The golden was captured in a separate process and committed
/// as an immutable fixture, so a match here proves the same seed yields the same
/// sequence across process runs (and, since the algorithm is portable, across
/// runtimes and architectures). A failure is a repeatability-contract event,
/// never a reason to regenerate the fixture.
/// </summary>
public sealed class DeterministicRandomGoldenTests
{
    private const int GoldenSeed = 42;

    [Fact]
    public void Seed42_ReproducesCapturedGolden()
    {
        ulong[] expected = LoadGolden("deterministic-random-seed42.txt");

        var random = new DeterministicRandom(GoldenSeed);
        var actual = new ulong[expected.Length];
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i] = random.NextULong();
        }

        Assert.Equal(expected, actual);
    }

    private static ulong[] LoadGolden(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Goldens", fileName);
        return File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => ulong.Parse(line.Trim(), System.Globalization.NumberStyles.HexNumber))
            .ToArray();
    }
}
