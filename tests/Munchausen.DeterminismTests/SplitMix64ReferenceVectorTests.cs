using Munchausen.Runtime;
using Xunit;

namespace Munchausen.DeterminismTests;

public sealed class SplitMix64ReferenceVectorTests
{
    // Published reference vector for SplitMix64 seeded with 0.
    // Source: skeeto/rng-go test suite (https://github.com/skeeto/rng-go).
    private static readonly ulong[] ExpectedSeedZero =
    [
        0xe220a8397b1dcdaf, 0x6e789e6aa1b965f4, 0x06c45d188009454f,
        0xf88bb8a8724c81ec, 0x1b39896a51a8749b, 0x53cb9f0c747ea2ea,
        0x2c829abe1f4532e1, 0xc584133ac916ab3c, 0x3ee5789041c98ac3,
        0xf3b8488c368cb0a6, 0x657eecdd3cb13d09, 0xc2d326e0055bdef6,
        0x8621a03fe0bbdb7b, 0x8e1f7555983aa92f, 0xb54e0f1600cc4d19,
    ];

    [Fact]
    public void Seed0_MatchesPublishedReferenceVector()
    {
        var generator = new SplitMix64(0);

        var actual = new ulong[ExpectedSeedZero.Length];
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i] = generator.Next();
        }

        Assert.Equal(ExpectedSeedZero, actual);
    }
}
