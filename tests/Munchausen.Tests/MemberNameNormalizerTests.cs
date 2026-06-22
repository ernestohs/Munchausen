using Munchausen.Inference;
using Xunit;

namespace Munchausen.Tests;

public sealed class MemberNameNormalizerTests
{
    [Theory]
    [InlineData("FirstName", "firstname")]
    [InlineData("first_name", "firstname")]
    [InlineData("FIRST-NAME", "firstname")]
    [InlineData("first-name", "firstname")]
    [InlineData("firstname", "firstname")]
    [InlineData("IPAddress", "ipaddress")]
    [InlineData("AddressLine1", "addressline1")]
    [InlineData("DOB", "dob")]
    [InlineData("isoCountryCode", "isocountrycode")]
    [InlineData("Customer_Email", "customeremail")]
    public void Normalize_StripsSeparatorsAndLowercases(string input, string expected)
    {
        Assert.Equal(expected, MemberNameNormalizer.Normalize(input));
    }
}
