using Munchausen;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class ExplainTests
{
    private static MemberInferenceReport Member(InferenceReport report, string name) =>
        report.Members.Single(m => m.MemberPath == name);

    [Fact]
    public void Explain_ReportsSourcesAndConfidence()
    {
        InferenceReport report = Lie.Define<Car>().Build().Explain();

        Assert.Equal("Type", Member(report, "Id").Source.ToString());          // Guid type default
        Assert.Equal("Semantic", Member(report, "Make").Source.ToString());    // Vehicle.Make
        Assert.Equal("High", Member(report, "Make").Confidence.ToString());
        Assert.Equal("ChildDefinition", Member(report, "Owner").Source.ToString());
    }

    [Fact]
    public void Explain_RejectedSemanticCandidate_FallsBackToType()
    {
        // Product.Code: short-code (Low) is rejected by the default Balanced mode.
        InferenceReport report = Lie.Define<Product>().Build().Explain();
        Assert.Equal("Type", Member(report, "Code").Source.ToString());
    }

    [Fact]
    public void Explain_OverriddenRules_AreReported()
    {
        InferenceReport report = Lie.Define<Car>()
            .With(car => car.Make, "first")
            .With(car => car.Make, "second")
            .Build()
            .Explain();

        MemberInferenceReport make = Member(report, "Make");
        Assert.Single(make.OverriddenRules);
    }

    [Fact]
    public void Explain_DerivedMember_ReportsDerivationOrder()
    {
        InferenceReport report = Lie.Define<Car>()
            .Derive(car => car.Year, (_, _) => 2020)
            .Build()
            .Explain();

        MemberInferenceReport year = Member(report, "Year");
        Assert.Equal("Derived", year.Source.ToString());
        Assert.Equal(0, year.DerivationOrder);
    }

    [Fact]
    public void Explain_RecursiveModel_ReportsSelfTypeAsSingleEntry()
    {
        InferenceReport report = Lie.Define<Employee>().Build().Explain();

        Assert.Single(report.Members, m => m.MemberPath == "Manager");
        Assert.Equal("ChildDefinition", Member(report, "Manager").Source.ToString());
    }

    [Fact]
    public void Explain_ToText_HasOneLinePerMember()
    {
        InferenceReport report = Lie.Define<Owner>().Build().Explain();
        string text = report.ToText();

        foreach (MemberInferenceReport member in report.Members)
        {
            Assert.Contains($"Owner.{member.MemberPath}", text);
        }
    }
}
