using System.Buffers.Binary;

namespace Munchausen.Runtime;

/// <summary>
/// The single source of randomness for a generation operation. Wraps an owned
/// <see cref="Xoshiro256StarStar"/> engine and exposes the distribution helpers
/// that back the public <c>RandomData</c> facade. All randomness in an operation
/// draws from one instance, consumed in traversal order, which is the
/// determinism contract.
/// </summary>
internal sealed class DeterministicRandom
{
    private const double UnitDoubleScale = 1.0 / (1UL << 53);
    private const string AlphaNumericChars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private Xoshiro256StarStar _engine;

    /// <summary>
    /// Seeds the engine from a user <see cref="int"/> seed. The seed is widened
    /// to 64 bits by sign extension before the SplitMix64 expansion; the exact
    /// mapping is an internal detail and seeded output is only stable within a
    /// library minor version.
    /// </summary>
    public DeterministicRandom(int seed)
        : this(unchecked((ulong)seed))
    {
    }

    public DeterministicRandom(ulong seed)
        => _engine = Xoshiro256StarStar.FromSeed(seed);

    internal DeterministicRandom(Xoshiro256StarStar engine)
        => _engine = engine;

    /// <summary>Raw 64-bit draw: the primitive every helper consumes.</summary>
    public ulong NextULong() => _engine.Next();

    public bool Bool(double probability = 0.5)
    {
        if (probability <= 0.0)
        {
            return false;
        }

        if (probability >= 1.0)
        {
            return true;
        }

        return NextUnitDouble() < probability;
    }

    public int Int(int minInclusive = int.MinValue, int maxInclusive = int.MaxValue)
    {
        if (minInclusive > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxInclusive),
                "maxInclusive must be greater than or equal to minInclusive.");
        }

        // (long) arithmetic keeps the +1 from overflowing even for the full int range.
        ulong span = (ulong)((long)maxInclusive - minInclusive) + 1;
        return unchecked((int)((long)minInclusive + (long)NextBounded(span)));
    }

    public long Long(long minInclusive = long.MinValue, long maxInclusive = long.MaxValue)
    {
        if (minInclusive > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxInclusive),
                "maxInclusive must be greater than or equal to minInclusive.");
        }

        ulong spanMinusOne = unchecked((ulong)(maxInclusive - minInclusive));
        if (spanMinusOne == ulong.MaxValue)
        {
            // Full 2^64 range: every draw is in range, no rejection needed.
            return unchecked((long)NextULong());
        }

        return unchecked(minInclusive + (long)NextBounded(spanMinusOne + 1));
    }

    public decimal Decimal(decimal minInclusive, decimal maxInclusive, int decimals = 2)
    {
        if (minInclusive > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxInclusive),
                "maxInclusive must be greater than or equal to minInclusive.");
        }

        if (decimals is < 0 or > 28)
        {
            throw new ArgumentOutOfRangeException(
                nameof(decimals), "decimals must be between 0 and 28.");
        }

        decimal value = minInclusive + ((maxInclusive - minInclusive) * NextUnitDecimal());
        value = Math.Round(value, decimals, MidpointRounding.ToEven);

        if (value < minInclusive)
        {
            return minInclusive;
        }

        return value > maxInclusive ? maxInclusive : value;
    }

    public double Double(double minInclusive = 0.0, double maxInclusive = 1.0)
    {
        if (minInclusive > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxInclusive),
                "maxInclusive must be greater than or equal to minInclusive.");
        }

        return minInclusive + ((maxInclusive - minInclusive) * NextUnitDouble());
    }

    public string String(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (length == 0)
        {
            return string.Empty;
        }

        return string.Create(length, this, static (span, random) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                // Printable ASCII: space (0x20) through tilde (0x7E).
                span[i] = (char)random.Int(0x20, 0x7E);
            }
        });
    }

    public string AlphaNumeric(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (length == 0)
        {
            return string.Empty;
        }

        return string.Create(length, this, static (span, random) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = AlphaNumericChars[random.Int(0, AlphaNumericChars.Length - 1)];
            }
        });
    }

    public byte[] Bytes(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var buffer = new byte[length];
        Fill(buffer);
        return buffer;
    }

    public Guid Guid()
    {
        Span<byte> bytes = stackalloc byte[16];
        Fill(bytes);
        return new Guid(bytes);
    }

    public T Pick<T>(IReadOnlyList<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Count == 0)
        {
            throw new ArgumentException("Cannot pick from an empty collection.", nameof(values));
        }

        return values[Int(0, values.Count - 1)];
    }

    public T Pick<T>(params T[] values) => Pick((IReadOnlyList<T>)values);

    public T Weighted<T>(IReadOnlyList<T> values, IReadOnlyList<double> weights)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(weights);
        if (values.Count == 0)
        {
            throw new ArgumentException("Cannot pick from an empty collection.", nameof(values));
        }

        if (values.Count != weights.Count)
        {
            throw new ArgumentException("values and weights must have the same length.", nameof(weights));
        }

        double total = 0.0;
        for (int i = 0; i < weights.Count; i++)
        {
            double weight = weights[i];
            if (!double.IsFinite(weight) || weight <= 0.0)
            {
                throw new ArgumentException(
                    "Weights must be finite and positive.", nameof(weights));
            }

            total += weight;
        }

        double target = NextUnitDouble() * total;
        double cumulative = 0.0;
        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (target < cumulative)
            {
                return values[i];
            }
        }

        // Floating-point rounding can leave target == total; fall back to the last.
        return values[values.Count - 1];
    }

    public IReadOnlyList<T> Sample<T>(IReadOnlyList<T> values, int count, bool replacement = false)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (count == 0)
        {
            return Array.Empty<T>();
        }

        if (replacement)
        {
            var withReplacement = new T[count];
            for (int i = 0; i < count; i++)
            {
                withReplacement[i] = values[Int(0, values.Count - 1)];
            }

            return withReplacement;
        }

        if (count > values.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "count cannot exceed the number of values when sampling without replacement.");
        }

        // Partial Fisher-Yates: draw distinct positions in index order.
        var pool = new T[values.Count];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = values[i];
        }

        var result = new T[count];
        for (int i = 0; i < count; i++)
        {
            int j = Int(i, pool.Length - 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
            result[i] = pool[i];
        }

        return result;
    }

    public TEnum Enum<TEnum>() where TEnum : struct, Enum
    {
        TEnum[] members = System.Enum.GetValues<TEnum>();
        if (members.Length == 0)
        {
            throw new ArgumentException($"Enum {typeof(TEnum)} has no members.");
        }

        return members[Int(0, members.Length - 1)];
    }

    private void Fill(Span<byte> destination)
    {
        int offset = 0;
        while (offset + sizeof(ulong) <= destination.Length)
        {
            BinaryPrimitives.WriteUInt64LittleEndian(destination[offset..], NextULong());
            offset += sizeof(ulong);
        }

        if (offset < destination.Length)
        {
            ulong remaining = NextULong();
            for (; offset < destination.Length; offset++)
            {
                destination[offset] = (byte)remaining;
                remaining >>= 8;
            }
        }
    }

    private double NextUnitDouble() => (NextULong() >> 11) * UnitDoubleScale;

    private decimal NextUnitDecimal()
    {
        // 128 bits drawn; the top 96 form a uniform fraction in [0, 1].
        UInt128 bits = ((UInt128)NextULong() << 64) | NextULong();
        UInt128 top96 = bits >> 32;
        var mantissa = new decimal(
            (int)(uint)top96,
            (int)(uint)(top96 >> 32),
            (int)(uint)(top96 >> 64),
            isNegative: false,
            scale: 0);

        // 2^96 - 1 == decimal.MaxValue, so this lands in [0, 1].
        return mantissa / decimal.MaxValue;
    }

    /// <summary>
    /// Returns a uniformly distributed value in <c>[0, range)</c> using Lemire's
    /// nearly-divisionless rejection method.
    /// </summary>
    private ulong NextBounded(ulong range)
    {
        ulong x = NextULong();
        UInt128 product = (UInt128)x * range;
        ulong low = (ulong)product;

        if (low < range)
        {
            ulong threshold = unchecked(0UL - range) % range;
            while (low < threshold)
            {
                x = NextULong();
                product = (UInt128)x * range;
                low = (ulong)product;
            }
        }

        return (ulong)(product >> 64);
    }
}
