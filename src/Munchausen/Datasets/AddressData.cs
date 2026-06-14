using System.Globalization;
using Munchausen.Datasets;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>Postal addresses and geography (English / United States style).</summary>
public sealed class AddressData
{
    private readonly DeterministicRandom _random;

    internal AddressData(DeterministicRandom random) => _random = random;

    /// <summary>A street address, e.g. <c>1742 Maple Street</c>.</summary>
    public string StreetAddress() =>
        $"{_random.Int(1, 9999)} {_random.Pick(EnData.StreetNames)} {_random.Pick(EnData.StreetSuffixes)}";

    /// <summary>A city name.</summary>
    public string City() => _random.Pick(EnData.Cities);

    /// <summary>A full state name, e.g. <c>Oregon</c>.</summary>
    public string State() => _random.Pick(EnData.States);

    /// <summary>A five-digit numeric postal code.</summary>
    public string PostalCode() => _random.Int(0, 99999).ToString("D5", CultureInfo.InvariantCulture);

    /// <summary>An English country short name. Draws independently of <see cref="CountryCode"/>.</summary>
    public string Country() => _random.Pick(EnData.Countries).Name;

    /// <summary>An ISO 3166-1 alpha-2 country code. Draws independently of <see cref="Country"/>.</summary>
    public string CountryCode() => _random.Pick(EnData.Countries).Code;

    /// <summary>A latitude in -90 to 90, six decimal places.</summary>
    public double Latitude() => Math.Round(_random.Double(-90, 90), 6);

    /// <summary>A longitude in -180 to 180, six decimal places.</summary>
    public double Longitude() => Math.Round(_random.Double(-180, 180), 6);
}
