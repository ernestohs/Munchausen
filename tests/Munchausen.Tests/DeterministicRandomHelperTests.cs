using Munchausen.Runtime;
using Xunit;

namespace Munchausen.Tests;

public sealed class DeterministicRandomHelperTests
{
    private enum Color
    {
        Red,
        Green,
        Blue,
    }

    private static DeterministicRandom Seeded() => new(0xC0FFEE);

    [Fact]
    public void Int_IsInclusiveOfBothBounds()
    {
        var random = Seeded();
        bool sawMin = false;
        bool sawMax = false;

        for (int i = 0; i < 1_000; i++)
        {
            int value = random.Int(1, 3);
            Assert.InRange(value, 1, 3);
            sawMin |= value == 1;
            sawMax |= value == 3;
        }

        Assert.True(sawMin, "minInclusive (1) was never produced.");
        Assert.True(sawMax, "maxInclusive (3) was never produced.");
    }

    [Fact]
    public void Int_DegenerateRange_ReturnsTheSingleValue()
    {
        var random = Seeded();
        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(7, random.Int(7, 7));
        }
    }

    [Fact]
    public void Int_FullRange_DoesNotThrow()
    {
        var random = Seeded();
        for (int i = 0; i < 100; i++)
        {
            _ = random.Int();
        }
    }

    [Fact]
    public void Int_MinGreaterThanMax_Throws()
    {
        var random = Seeded();
        Assert.Throws<ArgumentOutOfRangeException>(() => random.Int(5, 4));
    }

    [Fact]
    public void Long_IsInclusiveAndHandlesFullRange()
    {
        var random = Seeded();
        bool sawMin = false;
        bool sawMax = false;

        for (int i = 0; i < 1_000; i++)
        {
            long value = random.Long(10, 12);
            Assert.InRange(value, 10, 12);
            sawMin |= value == 10;
            sawMax |= value == 12;
        }

        Assert.True(sawMin && sawMax);

        // Full range must not throw or reject forever.
        for (int i = 0; i < 100; i++)
        {
            _ = random.Long();
        }
    }

    [Fact]
    public void Double_StaysWithinBounds()
    {
        var random = Seeded();
        for (int i = 0; i < 1_000; i++)
        {
            double value = random.Double(-2.0, 5.0);
            Assert.InRange(value, -2.0, 5.0);
        }
    }

    [Fact]
    public void Decimal_StaysWithinBoundsAtRequestedScale()
    {
        var random = Seeded();
        for (int i = 0; i < 1_000; i++)
        {
            decimal value = random.Decimal(0m, 100m, decimals: 2);
            Assert.InRange(value, 0m, 100m);
            Assert.Equal(value, Math.Round(value, 2));
        }
    }

    [Fact]
    public void Bool_ZeroAndOneAreDeterministic()
    {
        var random = Seeded();
        for (int i = 0; i < 100; i++)
        {
            Assert.False(random.Bool(0.0));
            Assert.True(random.Bool(1.0));
        }
    }

    [Fact]
    public void Bool_FairCoinProducesBothOutcomes()
    {
        var random = Seeded();
        bool sawTrue = false;
        bool sawFalse = false;

        for (int i = 0; i < 100; i++)
        {
            if (random.Bool())
            {
                sawTrue = true;
            }
            else
            {
                sawFalse = true;
            }
        }

        Assert.True(sawTrue && sawFalse);
    }

    [Fact]
    public void Weighted_NormalizesWeightsAndRespectsProportions()
    {
        var random = Seeded();
        var values = new[] { "a", "b", "c" };
        // Weights deliberately do not sum to one.
        var weights = new[] { 60.0, 30.0, 10.0 };
        var counts = new Dictionary<string, int> { ["a"] = 0, ["b"] = 0, ["c"] = 0 };

        const int draws = 20_000;
        for (int i = 0; i < draws; i++)
        {
            counts[random.Weighted(values, weights)]++;
        }

        Assert.True(counts["a"] > counts["b"]);
        Assert.True(counts["b"] > counts["c"]);
        // ~60% for "a"; generous tolerance to stay flake-free.
        Assert.InRange(counts["a"] / (double)draws, 0.55, 0.65);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Weighted_RejectsNonPositiveOrNonFiniteWeights(double badWeight)
    {
        var random = Seeded();
        var values = new[] { "a", "b" };
        var weights = new[] { 1.0, badWeight };

        Assert.Throws<ArgumentException>(() => random.Weighted(values, weights));
    }

    [Fact]
    public void Sample_WithoutReplacement_ProducesUniqueElements()
    {
        var random = Seeded();
        var values = Enumerable.Range(0, 20).ToArray();

        for (int trial = 0; trial < 200; trial++)
        {
            IReadOnlyList<int> sample = random.Sample(values, count: 5, replacement: false);
            Assert.Equal(5, sample.Count);
            Assert.Equal(sample.Count, sample.Distinct().Count());
            Assert.All(sample, value => Assert.Contains(value, values));
        }
    }

    [Fact]
    public void Sample_WithoutReplacement_CountExceedingPopulation_Throws()
    {
        var random = Seeded();
        var values = new[] { 1, 2, 3 };
        Assert.Throws<ArgumentOutOfRangeException>(() => random.Sample(values, count: 4));
    }

    [Fact]
    public void Sample_WithReplacement_AllowsRepeatsAndOversampling()
    {
        var random = Seeded();
        var values = new[] { 1, 2 };

        IReadOnlyList<int> sample = random.Sample(values, count: 10, replacement: true);

        Assert.Equal(10, sample.Count);
        Assert.All(sample, value => Assert.Contains(value, values));
    }

    [Fact]
    public void Pick_ReturnsAListedValue()
    {
        var random = Seeded();
        var values = new[] { "x", "y", "z" };
        for (int i = 0; i < 100; i++)
        {
            Assert.Contains(random.Pick(values), values);
        }
    }

    [Fact]
    public void Enum_ReturnsDefinedMembersOnly()
    {
        var random = Seeded();
        bool sawRed = false;
        bool sawBlue = false;

        for (int i = 0; i < 200; i++)
        {
            Color value = random.Enum<Color>();
            Assert.True(System.Enum.IsDefined(value));
            sawRed |= value == Color.Red;
            sawBlue |= value == Color.Blue;
        }

        Assert.True(sawRed && sawBlue);
    }

    [Fact]
    public void String_HasRequestedLengthAndIsPrintable()
    {
        var random = Seeded();
        string value = random.String(64);

        Assert.Equal(64, value.Length);
        Assert.All(value, c => Assert.InRange(c, (char)0x20, (char)0x7E));
        Assert.Equal(string.Empty, random.String(0));
    }

    [Fact]
    public void AlphaNumeric_HasRequestedLengthAndCharset()
    {
        var random = Seeded();
        string value = random.AlphaNumeric(64);

        Assert.Equal(64, value.Length);
        Assert.All(value, c => Assert.True(char.IsAsciiLetterOrDigit(c)));
    }

    [Fact]
    public void Bytes_HasRequestedLength()
    {
        var random = Seeded();
        Assert.Empty(random.Bytes(0));
        Assert.Equal(13, random.Bytes(13).Length);
        Assert.Equal(16, random.Bytes(16).Length);
    }

    [Fact]
    public void SameSeed_ProducesIdenticalHelperSequences()
    {
        var first = new DeterministicRandom(98765);
        var second = new DeterministicRandom(98765);

        for (int i = 0; i < 1_000; i++)
        {
            Assert.Equal(first.Int(0, 1_000_000), second.Int(0, 1_000_000));
            Assert.Equal(first.Double(), second.Double());
            Assert.Equal(first.Guid(), second.Guid());
        }
    }
}
