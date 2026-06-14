using System.Globalization;
using Munchausen.Datasets;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>Commerce data. Reached through <c>Dataset&lt;CommerceData&gt;()</c>.</summary>
public sealed class CommerceData
{
    private readonly DeterministicRandom _random;

    internal CommerceData(DeterministicRandom random) => _random = random;

    /// <summary>A price in the inclusive range, rounded half-even to <paramref name="decimals"/> places.</summary>
    public decimal Price(decimal minInclusive = 1m, decimal maxInclusive = 1000m, int decimals = 2)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minInclusive);
        if (minInclusive > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxInclusive), "maxInclusive must be greater than or equal to minInclusive.");
        }

        if (decimals is < 0 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(decimals), "decimals must be between 0 and 4.");
        }

        return _random.Decimal(minInclusive, maxInclusive, decimals);
    }

    /// <summary>A product name composed as adjective + material + noun.</summary>
    public string ProductName() =>
        $"{_random.Pick(EnData.ProductAdjectives)} {_random.Pick(EnData.ProductMaterials)} {_random.Pick(EnData.ProductNouns)}";

    /// <summary>A department/category noun.</summary>
    public string Category() => _random.Pick(EnData.Categories);

    /// <summary>A SKU in the <c>ABC-12345</c> pattern.</summary>
    public string Sku()
    {
        var letters = new char[3];
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i] = (char)_random.Int('A', 'Z');
        }

        return $"{new string(letters)}-{_random.Int(0, 99999).ToString("D5", CultureInfo.InvariantCulture)}";
    }

    /// <summary>An ISO 4217 alpha-3 currency code.</summary>
    public string CurrencyCode() => _random.Pick(EnData.CurrencyCodes);
}
