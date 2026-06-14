using Munchausen;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class GenerationDepthCycleTests
{
    [Fact]
    public void SelfReference_TerminatesAsNull()
    {
        LieDefinition<Node> definition = Lie.Define<Node>().Build();

        Node node = definition.Generate(new GenerationOptions { Seed = 1 });

        Assert.Null(node.Next); // path-based cycle at depth 1
    }

    [Fact]
    public void SiblingRepeatsOfSameType_AreBothGenerated()
    {
        LieDefinition<Pair> definition = Lie.Define<Pair>().Build();

        Pair pair = definition.Generate(new GenerationOptions { Seed = 1 });

        Assert.NotNull(pair.First);
        Assert.NotNull(pair.Second);
    }

    [Fact]
    public void MaximumDepth_TerminatesDistinctTypeChain()
    {
        LieDefinition<Outer> definition = Lie.Define<Outer>()
            .WithDefaults(new GenerationDefaults { MaximumDepth = 1 })
            .Build();

        Outer outer = definition.Generate(new GenerationOptions { Seed = 1 });

        Assert.NotNull(outer.Middle);        // depth 1 is allowed
        Assert.Null(outer.Middle!.Inner);    // depth 2 exceeds the limit
    }

    [Fact]
    public void SelfReferentialCollection_TerminatesEmpty()
    {
        LieDefinition<TreeNode> definition = Lie.Define<TreeNode>().Build();

        TreeNode tree = definition.Generate(new GenerationOptions { Seed = 1 });

        Assert.Empty(tree.Children);
    }

    [Fact]
    public void ThrowBehavior_RaisesGenerationExceptionOnCycle()
    {
        LieDefinition<Node> definition = Lie.Define<Node>()
            .WithDefaults(new GenerationDefaults { CycleBehavior = CycleBehavior.Throw })
            .Build();

        var exception = Assert.Throws<LieGenerationException>(
            () => definition.Generate(new GenerationOptions { Seed = 1 }));
        Assert.Equal(typeof(Node), exception.ModelType);
    }
}
