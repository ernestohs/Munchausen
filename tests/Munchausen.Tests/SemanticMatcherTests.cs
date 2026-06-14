using Munchausen.Inference;
using Xunit;

namespace Munchausen.Tests;

public sealed class SemanticMatcherTests
{
    private static ScoredCandidate Best(string member, string model, Type valueType)
    {
        IReadOnlyList<ScoredCandidate> scored = SemanticMatcher.Match(member, model, valueType);
        Assert.NotEmpty(scored);
        ScoredCandidate best = scored[0];
        foreach (ScoredCandidate candidate in scored)
        {
            if (Confidence.Rank(candidate.Confidence) > Confidence.Rank(best.Confidence))
            {
                best = candidate;
            }
        }

        return best;
    }

    [Fact]
    public void ExactMatchWithMatchingHint_IsHigh()
    {
        // The canonical Car.Make case.
        ScoredCandidate make = Best("Make", "Car", typeof(string));
        Assert.Equal(InferenceConfidence.High, make.Confidence);
        Assert.Equal("Vehicle.Make", make.Generator);
    }

    [Theory]
    [InlineData("Email", "User", "High")]       // no-hint base High
    [InlineData("IpAddress", "Server", "Medium")] // no-hint base Medium
    [InlineData("Code", "Thing", "Low")]        // no-hint base Low
    public void ExactMatchNoHint_UsesBaseConfidence(string member, string model, string expected)
    {
        Assert.Equal(expected, Best(member, model, typeof(string)).Confidence.ToString());
    }

    [Fact]
    public void HintGatedMiss_MakeAndModel_DropToLow()
    {
        // Per Option 1, the per-row note (no hint: Low) wins over rule #4's "one below".
        ScoredCandidate make = Best("Make", "Printer", typeof(string));
        Assert.Equal(InferenceConfidence.Low, make.Confidence);
        Assert.Equal("Vehicle.Make", make.Generator);

        Assert.Equal(InferenceConfidence.Low, Best("Model", "Printer", typeof(string)).Confidence);
    }

    [Fact]
    public void HintGatedMiss_Year_SwitchesToRecentYearAtMedium()
    {
        ScoredCandidate year = Best("Year", "Building", typeof(int));
        Assert.Equal(InferenceConfidence.Medium, year.Confidence);
        Assert.Equal("internal recent-year", year.Generator);
    }

    [Fact]
    public void SuffixMatch_IsOneLevelBelowExactResult()
    {
        // "CustomerEmail" ends with "email" (exact result High) -> suffix Medium.
        ScoredCandidate email = Best("CustomerEmail", "Order", typeof(string));
        Assert.Equal(InferenceConfidence.Medium, email.Confidence);
        Assert.Equal("Internet.Email", email.Generator);
    }

    [Fact]
    public void BareAddress_IsMediumWhileStreetAddressIsHigh()
    {
        Assert.Equal(InferenceConfidence.Medium, Best("Address", "Shipment", typeof(string)).Confidence);
        Assert.Equal(InferenceConfidence.High, Best("StreetAddress", "Shipment", typeof(string)).Confidence);
    }

    [Fact]
    public void ValueTypeMustMatch()
    {
        // "Price" is a decimal candidate; an int Price does not match it.
        Assert.Empty(SemanticMatcher.Match("Price", "Order", typeof(int)));
        Assert.NotEmpty(SemanticMatcher.Match("Price", "Order", typeof(decimal)));
    }
}
