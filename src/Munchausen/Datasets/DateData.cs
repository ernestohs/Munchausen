using Munchausen.Runtime;

namespace Munchausen;

/// <summary>
/// Dates relative to the operation's fixed reference time. Never reads the clock;
/// all values are UTC <see cref="DateTimeOffset"/>.
/// </summary>
public sealed class DateData
{
    private readonly DeterministicRandom _random;

    internal DateData(DeterministicRandom random, DateTimeOffset referenceTime)
    {
        _random = random;
        ReferenceTime = referenceTime;
    }

    /// <summary>The operation's fixed reference time.</summary>
    public DateTimeOffset ReferenceTime { get; }

    /// <summary>A uniform time in <c>[ReferenceTime - years, ReferenceTime]</c>.</summary>
    public DateTimeOffset Past(int years = 10)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(years);
        return Between(ReferenceTime.AddYears(-years), ReferenceTime);
    }

    /// <summary>A uniform time in <c>[ReferenceTime - days, ReferenceTime]</c>.</summary>
    public DateTimeOffset Recent(int days = 30)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days);
        return Between(ReferenceTime.AddDays(-days), ReferenceTime);
    }

    /// <summary>A uniform time in <c>[ReferenceTime, ReferenceTime + days]</c>.</summary>
    public DateTimeOffset Soon(int days = 30)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days);
        return Between(ReferenceTime, ReferenceTime.AddDays(days));
    }

    /// <summary>A uniform time in <c>[ReferenceTime, ReferenceTime + years]</c>.</summary>
    public DateTimeOffset Future(int years = 10)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(years);
        return Between(ReferenceTime, ReferenceTime.AddYears(years));
    }

    /// <summary>A uniform UTC time in the inclusive range.</summary>
    public DateTimeOffset Between(DateTimeOffset minInclusive, DateTimeOffset maxInclusive)
    {
        if (minInclusive > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxInclusive), "maxInclusive must be greater than or equal to minInclusive.");
        }

        long ticks = _random.Long(minInclusive.UtcTicks, maxInclusive.UtcTicks);
        return new DateTimeOffset(ticks, TimeSpan.Zero);
    }

    /// <summary>A birth date whose whole-year age at <see cref="ReferenceTime"/> is in the inclusive range.</summary>
    public DateTimeOffset BirthDate(int minAgeInclusive = 18, int maxAgeInclusive = 80)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minAgeInclusive);
        if (minAgeInclusive > maxAgeInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAgeInclusive), "maxAgeInclusive must be greater than or equal to minAgeInclusive.");
        }

        DateTimeOffset latest = ReferenceTime.AddYears(-minAgeInclusive);
        DateTimeOffset earliest = ReferenceTime.AddYears(-(maxAgeInclusive + 1)).AddDays(1);
        return Between(earliest, latest);
    }
}
