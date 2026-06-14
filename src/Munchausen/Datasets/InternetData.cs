using System.Globalization;
using System.Text;
using Munchausen.Datasets;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>
/// Internet identifiers. Contact data is fictional-safe: email domains are the
/// RFC 2606 reserved set; IPs come from the TEST-NET documentation blocks.
/// </summary>
public sealed class InternetData
{
    private static readonly string[] Separators = { ".", "_", string.Empty };
    private static readonly string[] TestNetBlocks = { "192.0.2", "198.51.100", "203.0.113" };

    private readonly DeterministicRandom _random;

    internal InternetData(DeterministicRandom random) => _random = random;

    /// <summary>An email address derived from internally drawn name parts.</summary>
    public string Email() => Email(_random.Pick(EnData.FirstNames), _random.Pick(EnData.LastNames));

    /// <summary>An email address derived from the supplied names, for coherent name/email pairs.</summary>
    public string Email(string firstName, string lastName)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);

        string local = $"{Clean(firstName)}{_random.Pick(Separators)}{Clean(lastName)}";
        if (_random.Bool(0.3))
        {
            local += _random.Int(0, 99).ToString(CultureInfo.InvariantCulture);
        }

        return $"{local}@{_random.Pick(EnData.EmailDomains)}";
    }

    /// <summary>A lowercase ASCII username, 5 to 16 characters.</summary>
    public string UserName() =>
        $"{Clean(_random.Pick(EnData.FirstNames))}{_random.Int(1000, 9999).ToString(CultureInfo.InvariantCulture)}";

    /// <summary>A bare registrable domain under the reserved <c>.example</c> TLD.</summary>
    public string DomainName() => $"{_random.Pick(EnData.LoremWords)}-{_random.Pick(EnData.LoremWords)}.example";

    /// <summary>An absolute <c>https</c> URL whose host is a generated domain.</summary>
    public string Url() => $"https://{DomainName()}";

    /// <summary>A syntactically valid IPv4 dotted-quad from the TEST-NET documentation blocks.</summary>
    public string IpAddress() => $"{_random.Pick(TestNetBlocks)}.{_random.Int(0, 255)}";

    private static string Clean(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (char c in value)
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                builder.Append(char.ToLowerInvariant(c));
            }
        }

        return builder.ToString();
    }
}
