using Munchausen.Runtime;

namespace Munchausen;

/// <summary>
/// Primitive random values and choices, drawn from the operation's deterministic
/// stream. Numeric bounds are inclusive. Semantic values live in the other datasets.
/// </summary>
public sealed class RandomData
{
    private readonly DeterministicRandom _random;

    internal RandomData(DeterministicRandom random) => _random = random;

    /// <summary>Returns true with the given probability (0 to 1).</summary>
    public bool Bool(double probability = 0.5) => _random.Bool(probability);

    /// <summary>A uniform integer in the inclusive range.</summary>
    public int Int(int minInclusive = int.MinValue, int maxInclusive = int.MaxValue) =>
        _random.Int(minInclusive, maxInclusive);

    /// <summary>A uniform long in the inclusive range.</summary>
    public long Long(long minInclusive = long.MinValue, long maxInclusive = long.MaxValue) =>
        _random.Long(minInclusive, maxInclusive);

    /// <summary>A uniform decimal in the inclusive range, rounded to <paramref name="decimals"/> places.</summary>
    public decimal Decimal(decimal minInclusive, decimal maxInclusive, int decimals = 2) =>
        _random.Decimal(minInclusive, maxInclusive, decimals);

    /// <summary>A uniform double in the inclusive range.</summary>
    public double Double(double minInclusive = 0, double maxInclusive = 1) =>
        _random.Double(minInclusive, maxInclusive);

    /// <summary>A string of printable characters of the given length.</summary>
    public string String(int length) => _random.String(length);

    /// <summary>A string of letters and digits of the given length.</summary>
    public string AlphaNumeric(int length) => _random.AlphaNumeric(length);

    /// <summary>A byte array of the given length.</summary>
    public byte[] Bytes(int length) => _random.Bytes(length);

    /// <summary>A random <see cref="System.Guid"/>.</summary>
    public Guid Guid() => _random.Guid();

    /// <summary>A uniformly chosen element.</summary>
    public T Pick<T>(IReadOnlyList<T> values) => _random.Pick(values);

    /// <summary>A uniformly chosen element.</summary>
    public T Pick<T>(params T[] values) => _random.Pick(values);

    /// <summary>An element chosen with probability proportional to its weight.</summary>
    public T Weighted<T>(IReadOnlyList<WeightedValue<T>> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var items = new T[values.Count];
        var weights = new double[values.Count];
        for (int i = 0; i < values.Count; i++)
        {
            items[i] = values[i].Value;
            weights[i] = values[i].Weight;
        }

        return _random.Weighted(items, weights);
    }

    /// <summary>A sample of <paramref name="count"/> elements, with or without replacement.</summary>
    public IReadOnlyList<T> Sample<T>(IReadOnlyList<T> values, int count, bool replacement = false) =>
        _random.Sample(values, count, replacement);

    /// <summary>A uniformly chosen value of the enum type.</summary>
    public TEnum Enum<TEnum>() where TEnum : struct, Enum => _random.Enum<TEnum>();
}
