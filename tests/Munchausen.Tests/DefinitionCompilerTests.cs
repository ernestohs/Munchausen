using Munchausen;
using Munchausen.Compilation;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class DefinitionCompilerTests
{
    private static IReadOnlyList<string> Codes(LieDefinitionException exception) =>
        exception.Diagnostics.Select(d => d.Code).ToList();

    [Fact]
    public void LIE001_InvalidMemberExpression()
    {
        var exception = Assert.Throws<LieDefinitionException>(
            () => Lie.Define<Car>().With(car => car.Owner.FirstName, "x").Build());
        Assert.Contains("LIE001", Codes(exception));
    }

    [Fact]
    public void LIE002_ConflictingMemberRules()
    {
        var exception = Assert.Throws<LieDefinitionException>(
            () => Lie.Define<Car>().With(car => car.Make, "x").Ignore(car => car.Make).Build());
        Assert.Contains("LIE002", Codes(exception));
    }

    [Fact]
    public void LIE003_UnresolvedRequiredMember()
    {
        var exception = Assert.Throws<LieDefinitionException>(
            () => Lie.Define<RequiredUnsupportedModel>().Build());
        Assert.Contains("LIE003", Codes(exception));
    }

    [Fact]
    public void LIE004_AmbiguousConstructor()
    {
        var exception = Assert.Throws<LieDefinitionException>(
            () => Lie.Define<AmbiguousCtorModel>().Build());
        Assert.Contains("LIE004", Codes(exception));
    }

    [Fact]
    public void LIE005_InvalidOption_EmptyName()
    {
        var exception = Assert.Throws<LieDefinitionException>(
            () => Lie.Define<Car>().WithName("   ").Build());
        Assert.Contains("LIE005", Codes(exception));
    }

    [Fact]
    public void MultipleErrors_AreAllReported_WithMessageContract()
    {
        var exception = Assert.Throws<LieDefinitionException>(() =>
            Lie.Define<Car>()
                .WithName("  ")                            // LIE005
                .With(car => car.Owner.FirstName, "x")     // LIE001
                .With(car => car.Make, "v")
                .Ignore(car => car.Make)                   // LIE002
                .Build());

        var errors = exception.Diagnostics
            .Where(d => d.Severity == LieDiagnosticSeverity.Error)
            .ToList();
        Assert.True(errors.Count >= 3);
        Assert.Contains("LIE005", errors.Select(d => d.Code));
        Assert.Contains("LIE001", errors.Select(d => d.Code));
        Assert.Contains("LIE002", errors.Select(d => d.Code));

        // Message contract: model-prefixed first error + remaining count.
        Assert.StartsWith("Definition for Car is invalid:", exception.Message);
        Assert.Contains("more error", exception.Message);
        Assert.Contains("see Diagnostics.", exception.Message);
    }

    [Fact]
    public void ValidDefinition_BuildsAndExposesName()
    {
        LieDefinition<Car> definition = Lie.Define<Car>().WithName("cars").Build();
        Assert.Equal("cars", definition.Name);
    }

    [Fact]
    public void RecursiveModel_CompilesOnceAndTerminates()
    {
        LieDefinition<Employee> definition = Lie.Define<Employee>().Build();

        Assert.Contains(typeof(Employee), definition.Plan.ReachablePlans.Keys);

        MemberPlan manager = definition.Plan.Root.Members.Single(m => m.Member.Name == "Manager");
        NestedSource nested = Assert.IsType<NestedSource>(manager.Source);
        Assert.Equal(typeof(Employee), nested.ChildType);

        // Self-type reference is reported as an Info diagnostic, not an error.
        Assert.Contains(definition.Plan.BuildDiagnostics, d => d.Code == "LIE009");
    }
}
