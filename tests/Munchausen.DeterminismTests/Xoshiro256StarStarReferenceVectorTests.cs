using Munchausen.Runtime;
using Xunit;

namespace Munchausen.DeterminismTests;

public sealed class Xoshiro256StarStarReferenceVectorTests
{
    // Published reference vector for xoshiro256** with state {1, 2, 3, 4}.
    // Source: skeeto/rng-go test suite (https://github.com/skeeto/rng-go).
    // The first output is hand-verifiable: rotl(2*5, 7) * 9 = 1280 * 9 = 11520 = 0x2d00.
    private static readonly ulong[] ExpectedState1234 =
    [
        0x0000000000002d00, 0x0000000000000000, 0x000000005a007080,
        0x10e0000000009d80, 0x10e0b61ce1009d80, 0x0870021ce143ad00,
        0xe071c3c2e143f089, 0x75a1690ef7a20380, 0x9309685b465c23f9,
        0x284f3cc2e13e3c88, 0xc8d749005a413820, 0x1194b410fef20904,
        0xb54a54470263b28c, 0x959e65495daf641c, 0xe561ccecea17f527,
    ];

    [Fact]
    public void State1234_MatchesPublishedReferenceVector()
    {
        var engine = new Xoshiro256StarStar(1, 2, 3, 4);

        var actual = new ulong[ExpectedState1234.Length];
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i] = engine.Next();
        }

        Assert.Equal(ExpectedState1234, actual);
    }

    [Fact]
    public void FirstOutput_IsHandVerifiableValue()
    {
        var engine = new Xoshiro256StarStar(1, 2, 3, 4);

        Assert.Equal(11520UL, engine.Next());
    }
}
