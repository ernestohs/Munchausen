using Munchausen;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class GenerationLifecycleTests
{
    [Fact]
    public void Lifecycle_ConstructsThenPopulatesInDeclarationOrderThenDerivesInRegistrationOrder()
    {
        var log = new List<string>();
        LieDefinition<LifecycleModel> definition = Lie.Define<LifecycleModel>()
            .ConstructWith(_ => { log.Add("ctor"); return new LifecycleModel(); })
            // Registered B, A, C — population must still run in declaration order A, B, C.
            .With(m => m.B, _ => { log.Add("B"); return 2; })
            .With(m => m.A, _ => { log.Add("A"); return 1; })
            .With(m => m.C, _ => { log.Add("C"); return 3; })
            // Registered E before D — derivations must run in registration order E, D.
            .Derive(m => m.E, (_, _) => { log.Add("E"); return 5; })
            .Derive(m => m.D, (_, _) => { log.Add("D"); return 4; })
            .Build();

        LifecycleModel result = definition.Generate();

        Assert.Equal(new[] { "ctor", "A", "B", "C", "E", "D" }, log);
        Assert.Equal((1, 2, 3, 4, 5), (result.A, result.B, result.C, result.D, result.E));
    }

    [Fact]
    public void SameSeed_ProducesEquivalentGraphs()
    {
        LieDefinition<Primitives> definition = Lie.Define<Primitives>().Build();

        Primitives first = definition.Generate(new GenerationOptions { Seed = 42 });
        Primitives second = definition.Generate(new GenerationOptions { Seed = 42 });

        Assert.Equal(
            (first.Number, first.Flag, first.Id, first.Ratio, first.Big),
            (second.Number, second.Flag, second.Id, second.Ratio, second.Big));
    }

    [Fact]
    public void SingleGenerate_MatchesFirstOfBatch()
    {
        LieDefinition<Primitives> definition = Lie.Define<Primitives>().Build();

        Primitives single = definition.Generate(new GenerationOptions { Seed = 99 });
        IReadOnlyList<Primitives> batch = definition.Generate(3, new GenerationOptions { Seed = 99 });

        Assert.Equal(single.Id, batch[0].Id);
        Assert.Equal(single.Number, batch[0].Number);
    }

    [Fact]
    public void Index_AdvancesAcrossBatch()
    {
        var indices = new List<long>();
        LieDefinition<Primitives> definition = Lie.Define<Primitives>()
            .With(m => m.Number, context => { indices.Add(context.Index); return 0; })
            .Build();

        definition.Generate(3, new GenerationOptions { Seed = 1 });

        Assert.Equal(new long[] { 0, 1, 2 }, indices);
    }

    [Fact]
    public void Collections_MaterializeToDeclaredShapes()
    {
        LieDefinition<IntCollections> definition = Lie.Define<IntCollections>().Build();

        IntCollections result = definition.Generate(new GenerationOptions { Seed = 1 });

        Assert.IsType<List<int>>(result.Numbers);
        Assert.IsType<int[]>(result.ArrayValues);
        Assert.InRange(result.Numbers.Count, 1, 3);
        Assert.InRange(result.ArrayValues.Length, 1, 3);
        Assert.InRange(result.ReadOnlyValues.Count, 1, 3);
    }

}
