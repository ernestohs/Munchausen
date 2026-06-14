namespace Munchausen;

/// <summary>An inclusive size range for a generated collection.</summary>
public readonly record struct CollectionSize
{
    private CollectionSize(int minCount, int maxCount)
    {
        MinCount = minCount;
        MaxCount = maxCount;
    }

    /// <summary>The inclusive minimum element count.</summary>
    public int MinCount { get; }

    /// <summary>The inclusive maximum element count.</summary>
    public int MaxCount { get; }

    /// <summary>A range that always produces exactly <paramref name="count"/> elements.</summary>
    public static CollectionSize Exactly(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return new CollectionSize(count, count);
    }

    /// <summary>A range between <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>, inclusive.</summary>
    public static CollectionSize Between(int minInclusive, int maxInclusive)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minInclusive);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxInclusive, minInclusive);
        return new CollectionSize(minInclusive, maxInclusive);
    }
}
