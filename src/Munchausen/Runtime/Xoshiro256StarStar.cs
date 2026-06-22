namespace Munchausen.Runtime;

/// <summary>
/// xoshiro256** pseudo-random generator. Reference algorithm by David Blackman
/// and Sebastiano Vigna (public domain), https://prng.di.unimi.it/xoshiro256starstar.c.
/// The seeded byte stream is identical on every runtime and architecture, which
/// is what makes Munchausen's repeatability contract portable.
/// </summary>
internal struct Xoshiro256StarStar
{
    private ulong _s0;
    private ulong _s1;
    private ulong _s2;
    private ulong _s3;

    public Xoshiro256StarStar(ulong s0, ulong s1, ulong s2, ulong s3)
    {
        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;
    }

    /// <summary>
    /// Builds an engine whose four state words come from the published
    /// SplitMix64 expansion of <paramref name="seed"/>.
    /// </summary>
    public static Xoshiro256StarStar FromSeed(ulong seed)
    {
        var expander = new SplitMix64(seed);
        return new Xoshiro256StarStar(
            expander.Next(),
            expander.Next(),
            expander.Next(),
            expander.Next());
    }

    public ulong Next()
    {
        ulong result = unchecked(Rotl(_s1 * 5, 7) * 9);
        ulong t = _s1 << 17;

        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;
        _s2 ^= t;
        _s3 = Rotl(_s3, 45);

        return result;
    }

    private static ulong Rotl(ulong x, int k) => (x << k) | (x >> (64 - k));
}
