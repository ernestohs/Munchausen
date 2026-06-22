using Munchausen.Datasets;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>Vehicle data. Reached through <c>Dataset&lt;VehicleData&gt;()</c>.</summary>
public sealed class VehicleData
{
    // VIN alphabet excludes I, O, Q.
    private const string VinAlphabet = "0123456789ABCDEFGHJKLMNPRSTUVWXYZ";
    private static readonly int[] Weights = { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };

    private readonly DeterministicRandom _random;
    private readonly int _referenceYear;

    internal VehicleData(DeterministicRandom random, DateTimeOffset referenceTime)
    {
        _random = random;
        _referenceYear = referenceTime.Year;
    }

    /// <summary>A manufacturer.</summary>
    public string Make() => _random.Pick(EnData.VehicleMakes);

    /// <summary>A model from any make.</summary>
    public string Model() => _random.Pick(EnData.VehicleModels);

    /// <summary>A model; an unknown make falls back to a plausible generic model.</summary>
    public string Model(string make)
    {
        ArgumentNullException.ThrowIfNull(make);
        return Model();
    }

    /// <summary>A model year from 1990 to the reference year plus one.</summary>
    public int Year() => _random.Int(1990, _referenceYear + 1);

    /// <summary>A 17-character VIN (no I/O/Q) with a valid check digit.</summary>
    public string Vin()
    {
        var characters = new char[17];
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = VinAlphabet[_random.Int(0, VinAlphabet.Length - 1)];
        }

        characters[8] = ComputeCheckDigit(characters);
        return new string(characters);
    }

    private static char ComputeCheckDigit(char[] characters)
    {
        int sum = 0;
        for (int i = 0; i < characters.Length; i++)
        {
            sum += Transliterate(characters[i]) * Weights[i];
        }

        int remainder = sum % 11;
        return remainder == 10 ? 'X' : (char)('0' + remainder);
    }

    private static int Transliterate(char c)
    {
        if (c is >= '0' and <= '9')
        {
            return c - '0';
        }

        // A=1..H=8, J=1..R=9, S=2..Z=9 (I, O, Q excluded).
        return c switch
        {
            'A' or 'J' => 1,
            'B' or 'K' or 'S' => 2,
            'C' or 'L' or 'T' => 3,
            'D' or 'M' or 'U' => 4,
            'E' or 'N' or 'V' => 5,
            'F' or 'W' => 6,
            'G' or 'P' or 'X' => 7,
            'H' or 'Y' => 8,
            'R' or 'Z' => 9,
            _ => 0,
        };
    }
}
