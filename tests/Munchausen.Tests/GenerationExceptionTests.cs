using Munchausen;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class GenerationExceptionTests
{
    [Fact]
    public void WithGeneratorThrows_WrappedOnceAsMemberPopulation()
    {
        var inner = new InvalidOperationException("boom");
        LieDefinition<Primitives> definition = Lie.Define<Primitives>()
            .With(m => m.Number, _ => throw inner)
            .Build();

        var exception = Assert.Throws<LieGenerationException>(() => definition.Generate());

        Assert.Equal(GenerationPhase.MemberPopulation, exception.Phase);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal(typeof(Primitives), exception.ModelType);
        Assert.Equal("Number", exception.MemberPath);
    }

    [Fact]
    public void DeriveThrows_WrappedOnceAsDerivation()
    {
        var inner = new InvalidOperationException("boom");
        LieDefinition<Primitives> definition = Lie.Define<Primitives>()
            .Derive(m => m.Number, (_, _) => throw inner)
            .Build();

        var exception = Assert.Throws<LieGenerationException>(() => definition.Generate());

        Assert.Equal(GenerationPhase.Derivation, exception.Phase);
        Assert.Same(inner, exception.InnerException);
        Assert.Equal("Number", exception.MemberPath);
    }

    [Fact]
    public void ConstructWithThrows_WrappedOnceAsConstruction()
    {
        var inner = new InvalidOperationException("boom");
        LieDefinition<Primitives> definition = Lie.Define<Primitives>()
            .ConstructWith(_ => throw inner)
            .Build();

        var exception = Assert.Throws<LieGenerationException>(() => definition.Generate());

        Assert.Equal(GenerationPhase.Construction, exception.Phase);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void OperationCanceledException_PassesThroughUnwrapped()
    {
        LieDefinition<Primitives> definition = Lie.Define<Primitives>()
            .With(m => m.Number, _ => throw new OperationCanceledException())
            .Build();

        Assert.Throws<OperationCanceledException>(() => definition.Generate());
    }
}
