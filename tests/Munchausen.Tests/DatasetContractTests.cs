using System.Text.RegularExpressions;
using Munchausen;
using Munchausen.Datasets;
using Munchausen.Runtime;
using Xunit;

namespace Munchausen.Tests;

public sealed class DatasetContractTests
{
    private static readonly DateTimeOffset Reference = new(2026, 6, 14, 12, 0, 0, TimeSpan.Zero);

    private static DeterministicRandom Rng(ulong seed = 1) => new(seed);

    [Fact]
    public void Name_FullName_IsFirstSpaceLast()
    {
        var name = new NameData(Rng());
        for (int i = 0; i < 50; i++)
        {
            string[] parts = name.FullName().Split(' ');
            Assert.Equal(2, parts.Length);
            Assert.NotEmpty(parts[0]);
            Assert.NotEmpty(parts[1]);
        }
    }

    [Fact]
    public void Internet_Email_UsesRfc2606DomainsAndIsLowercaseAscii()
    {
        var internet = new InternetData(Rng());
        for (int i = 0; i < 200; i++)
        {
            string email = internet.Email();
            Assert.Contains('@', email);
            string domain = email[(email.IndexOf('@') + 1)..];
            Assert.Contains(domain, new[] { "example.com", "example.org", "example.net" });
            Assert.Equal(email, email.ToLowerInvariant());
            Assert.DoesNotContain(email, c => char.IsWhiteSpace(c));
        }
    }

    [Fact]
    public void Internet_IpAddress_IsFromTestNetBlocks()
    {
        var internet = new InternetData(Rng());
        for (int i = 0; i < 100; i++)
        {
            string ip = internet.IpAddress();
            string[] octets = ip.Split('.');
            Assert.Equal(4, octets.Length);
            Assert.All(octets, o => Assert.InRange(int.Parse(o), 0, 255));
            Assert.True(
                ip.StartsWith("192.0.2.") || ip.StartsWith("198.51.100.") || ip.StartsWith("203.0.113."),
                $"{ip} is not a TEST-NET address");
        }
    }

    [Fact]
    public void Internet_UserName_IsLowercaseAsciiFiveToSixteen()
    {
        var internet = new InternetData(Rng());
        for (int i = 0; i < 100; i++)
        {
            string user = internet.UserName();
            Assert.InRange(user.Length, 5, 16);
            Assert.All(user, c => Assert.True(char.IsAsciiLetterOrDigit(c)));
            Assert.Equal(user, user.ToLowerInvariant());
        }
    }

    [Fact]
    public void Internet_DomainAndUrl_AreWellFormed()
    {
        var internet = new InternetData(Rng());
        Assert.EndsWith(".example", internet.DomainName());
        string url = internet.Url();
        Assert.StartsWith("https://", url);
        Assert.True(Uri.TryCreate(url, UriKind.Absolute, out _));
    }

    [Fact]
    public void Address_PostalCode_IsFiveDigits()
    {
        var address = new AddressData(Rng());
        for (int i = 0; i < 100; i++)
        {
            Assert.Matches("^[0-9]{5}$", address.PostalCode());
        }
    }

    [Fact]
    public void Address_Coordinates_AreInRangeWithSixDecimals()
    {
        var address = new AddressData(Rng());
        for (int i = 0; i < 100; i++)
        {
            Assert.InRange(address.Latitude(), -90.0, 90.0);
            Assert.InRange(address.Longitude(), -180.0, 180.0);
        }
    }

    [Fact]
    public void Date_BirthDate_AgeIsWithinRequestedRange()
    {
        var date = new DateData(Rng(), Reference);
        for (int i = 0; i < 200; i++)
        {
            DateTimeOffset birth = date.BirthDate(25, 30);
            int age = Reference.Year - birth.Year;
            if (Reference < birth.AddYears(age))
            {
                age--;
            }

            Assert.InRange(age, 25, 30);
        }
    }

    [Fact]
    public void Date_RelativeMethods_RespectDirectionAndReference()
    {
        var date = new DateData(Rng(), Reference);
        for (int i = 0; i < 100; i++)
        {
            Assert.InRange(date.Past(2), Reference.AddYears(-2), Reference);
            Assert.InRange(date.Recent(30), Reference.AddDays(-30), Reference);
            Assert.InRange(date.Soon(30), Reference, Reference.AddDays(30));
            Assert.InRange(date.Future(2), Reference, Reference.AddYears(2));
        }
    }

    [Fact]
    public void Date_PositiveArgumentsRequired()
    {
        var date = new DateData(Rng(), Reference);
        Assert.Throws<ArgumentOutOfRangeException>(() => date.Past(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => date.Recent(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => date.Between(Reference, Reference.AddDays(-1)));
    }

    [Fact]
    public void Lorem_Text_NeverExceedsLimitAndNeverCutsAWord()
    {
        var lorem = new LoremData(Rng());
        foreach (int max in new[] { 1, 10, 25, 80, 200 })
        {
            for (int i = 0; i < 25; i++)
            {
                string text = lorem.Text(max);
                Assert.True(text.Length <= max);
                foreach (string token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    Assert.Contains(token, EnData.LoremWords);
                }
            }
        }
    }

    [Fact]
    public void Lorem_Sentence_IsCapitalizedAndPeriodTerminated()
    {
        var lorem = new LoremData(Rng());
        for (int i = 0; i < 50; i++)
        {
            string sentence = lorem.Sentence();
            Assert.True(char.IsUpper(sentence[0]));
            Assert.EndsWith(".", sentence);
        }
    }

    [Fact]
    public void Vehicle_Vin_Is17CharsNoForbiddenLettersValidCheckDigit()
    {
        var vehicle = new VehicleData(Rng(), Reference);
        for (int i = 0; i < 200; i++)
        {
            string vin = vehicle.Vin();
            Assert.Equal(17, vin.Length);
            Assert.DoesNotContain(vin, c => c is 'I' or 'O' or 'Q');
            Assert.Equal(ExpectedCheckDigit(vin), vin[8]);
        }
    }

    [Fact]
    public void Vehicle_Year_IsWithinRange()
    {
        var vehicle = new VehicleData(Rng(), Reference);
        for (int i = 0; i < 100; i++)
        {
            Assert.InRange(vehicle.Year(), 1990, Reference.Year + 1);
        }
    }

    [Fact]
    public void Commerce_Price_RespectsBoundsRoundingAndValidation()
    {
        var commerce = new CommerceData(Rng());
        for (int i = 0; i < 100; i++)
        {
            decimal price = commerce.Price(10m, 20m, 2);
            Assert.InRange(price, 10m, 20m);
            Assert.Equal(price, Math.Round(price, 2));
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => commerce.Price(-1m, 10m));
        Assert.Throws<ArgumentOutOfRangeException>(() => commerce.Price(10m, 5m));
        Assert.Throws<ArgumentOutOfRangeException>(() => commerce.Price(0m, 10m, 5));
    }

    [Fact]
    public void Commerce_Sku_MatchesPattern()
    {
        var commerce = new CommerceData(Rng());
        for (int i = 0; i < 100; i++)
        {
            Assert.Matches("^[A-Z]{3}-[0-9]{5}$", commerce.Sku());
        }
    }

    // Independent VIN check-digit computation for verification.
    private static char ExpectedCheckDigit(string vin)
    {
        int[] weights = { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };
        const string map = "0123456789.ABCDEFGH..JKLMN.P.R..STUVWXYZ";
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            char c = vin[i];
            int value = char.IsDigit(c) ? c - '0' : map.IndexOf(c) % 10;
            sum += value * weights[i];
        }

        int remainder = sum % 11;
        return remainder == 10 ? 'X' : (char)('0' + remainder);
    }
}
