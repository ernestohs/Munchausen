using Munchausen;
using Munchausen.Builder;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.Tests;

public sealed class BuilderStateTests
{
    [Fact]
    public void WithSeed_IsEquivalentToWithDefaultsSeed()
    {
        LieDefinitionBuilder<Car> viaSeed = Lie.Define<Car>().WithSeed(42);
        LieDefinitionBuilder<Car> viaDefaults =
            Lie.Define<Car>().WithDefaults(new GenerationDefaults { Seed = 42 });

        Assert.Equal(42, viaSeed.State.Defaults.Seed);
        Assert.Equal(viaDefaults.State.Defaults.Seed, viaSeed.State.Defaults.Seed);
    }

    [Fact]
    public void WithDefaults_LastWriteWinsPerProperty()
    {
        LieDefinitionBuilder<Car> builder = Lie.Define<Car>()
            .WithDefaults(new GenerationDefaults { Seed = 1, MaximumDepth = 9 })
            .WithDefaults(new GenerationDefaults { Seed = 2 });

        Assert.Equal(2, builder.State.Defaults.Seed);          // overwritten
        Assert.Equal(9, builder.State.Defaults.MaximumDepth);  // untouched
    }

    [Fact]
    public void WithDefaults_NullPropertyIsNoOpinion()
    {
        LieDefinitionBuilder<Car> builder = Lie.Define<Car>()
            .WithDefaults(new GenerationDefaults { CycleBehavior = CycleBehavior.Throw })
            .WithDefaults(new GenerationDefaults { SemanticInference = SemanticInferenceMode.Aggressive });

        Assert.Equal(CycleBehavior.Throw, builder.State.Defaults.CycleBehavior);
        Assert.Equal(SemanticInferenceMode.Aggressive, builder.State.Defaults.SemanticInference);
    }

    [Fact]
    public void MemberRules_CaptureKindAndRegistrationOrder()
    {
        LieDefinitionBuilder<Car> builder = Lie.Define<Car>()
            .With(car => car.Make, "Saab")
            .Ignore(car => car.Price)
            .Derive(car => car.Year, (context, car) => 1989);

        IReadOnlyList<MemberRuleRecord> rules = builder.State.MemberRules;

        Assert.Equal(3, rules.Count);
        Assert.Equal(MemberRuleKind.WithValue, rules[0].Kind);
        Assert.Equal(MemberRuleKind.Ignore, rules[1].Kind);
        Assert.Equal(MemberRuleKind.Derive, rules[2].Kind);
        Assert.Equal(new[] { 0, 1, 2 }, rules.Select(rule => rule.RegistrationIndex));
    }

    [Fact]
    public void WithName_And_ConstructWith_AreCaptured()
    {
        LieDefinitionBuilder<Car> builder = Lie.Define<Car>()
            .WithName("cars")
            .ConstructWith(context => new Car());

        Assert.Equal("cars", builder.State.Name);
        Assert.NotNull(builder.State.Construction);
    }
}
