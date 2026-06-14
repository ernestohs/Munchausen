using Munchausen;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class GenerationCancellationTests
{
    [Fact]
    public void PreCancelledToken_ThrowsBeforeGenerating()
    {
        LieDefinition<Primitives> definition = Lie.Define<Primitives>().Build();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => definition.Generate(null, cts.Token));
    }

    [Fact]
    public void CancellationCheckpoint_BeforeNestedObject()
    {
        using var cts = new CancellationTokenSource();
        LieDefinition<CancelNested> definition = Lie.Define<CancelNested>()
            .With(m => m.Trigger, _ => { cts.Cancel(); return 0; })
            .Build();

        Assert.Throws<OperationCanceledException>(() => definition.Generate(null, cts.Token));
    }

    [Fact]
    public void CancellationCheckpoint_BeforeCollection()
    {
        using var cts = new CancellationTokenSource();
        LieDefinition<CancelCollection> definition = Lie.Define<CancelCollection>()
            .With(m => m.Trigger, _ => { cts.Cancel(); return 0; })
            .Build();

        Assert.Throws<OperationCanceledException>(() => definition.Generate(null, cts.Token));
    }

    [Fact]
    public void CancellationCheckpoint_BeforeDerivations()
    {
        using var cts = new CancellationTokenSource();
        LieDefinition<CancelDerive> definition = Lie.Define<CancelDerive>()
            .With(m => m.Trigger, _ => { cts.Cancel(); return 0; })
            .Derive(m => m.Derived, (_, _) => 1)
            .Build();

        Assert.Throws<OperationCanceledException>(() => definition.Generate(null, cts.Token));
    }

    [Fact]
    public void CancellationCheckpoint_BeforeEachRootObject()
    {
        using var cts = new CancellationTokenSource();
        int produced = 0;
        LieDefinition<Primitives> definition = Lie.Define<Primitives>()
            .With(m => m.Number, _ => { if (++produced >= 3) cts.Cancel(); return 0; })
            .Build();

        Assert.Throws<OperationCanceledException>(() => definition.Generate(100, null, cts.Token));
        Assert.Equal(3, produced);
    }
}
