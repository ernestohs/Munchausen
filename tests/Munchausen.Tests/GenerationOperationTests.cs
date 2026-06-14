using Munchausen;
using Munchausen.Compilation;
using Munchausen.Runtime;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

/// <summary>Internal tests for reference-time and seed resolution order.</summary>
public sealed class GenerationOperationTests
{
    private static GenerationPlan Plan() => Lie.Define<Primitives>().Build().Plan;

    [Fact]
    public void ReferenceTime_PrefersOptionsOverDefaults()
    {
        var optionsTime = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
        GenerationPlan plan = Lie.Define<Primitives>()
            .WithDefaults(new GenerationDefaults
            {
                ReferenceTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
            })
            .Build().Plan;

        var operation = new GenerationOperation(plan, new GenerationOptions { ReferenceTime = optionsTime }, default);

        Assert.Equal(optionsTime, operation.ReferenceTime);
    }

    [Fact]
    public void ReferenceTime_FallsBackToDefaultsThenTimeProvider()
    {
        var defaultsTime = new DateTimeOffset(2001, 2, 3, 0, 0, 0, TimeSpan.Zero);
        GenerationPlan withDefault = Lie.Define<Primitives>()
            .WithDefaults(new GenerationDefaults { ReferenceTime = defaultsTime })
            .Build().Plan;
        Assert.Equal(defaultsTime, new GenerationOperation(withDefault, null, default).ReferenceTime);

        var providerTime = new DateTimeOffset(2002, 3, 4, 0, 0, 0, TimeSpan.Zero);
        var timeProvider = new FixedTimeProvider(providerTime);
        Assert.Equal(
            providerTime,
            new GenerationOperation(Plan(), new GenerationOptions { TimeProvider = timeProvider }, default).ReferenceTime);
    }

    [Fact]
    public void Seed_FromOptionsOrDefaultsProducesIdenticalStreams()
    {
        GenerationPlan plan = Plan();
        var fromOptions = new GenerationOperation(plan, new GenerationOptions { Seed = 7 }, default);

        GenerationPlan withDefaultSeed = Lie.Define<Primitives>().WithSeed(7).Build().Plan;
        var fromDefaults = new GenerationOperation(withDefaultSeed, null, default);

        Assert.Equal(fromOptions.Random.NextULong(), fromDefaults.Random.NextULong());
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
