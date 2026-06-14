using Munchausen.Inference;
using Munchausen.Metadata;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class InferenceModeTests
{
    private static ResolvedSource Resolve<T>(string memberName, SemanticInferenceMode mode)
    {
        ModelMetadata metadata = new ReflectionModelMetadataProvider().GetMetadata(typeof(T));
        MemberMetadata member = metadata.Members.Single(m => m.Name == memberName);
        var context = new MemberInferenceContext(typeof(T), member, memberName, mode);
        return new InferenceEngine().Resolve(context);
    }

    [Fact]
    public void ProductCode_IsRejectedByBalancedAndFallsBackToType()
    {
        ResolvedSource result = Resolve<Product>("Code", SemanticInferenceMode.Balanced);

        Assert.Equal(InferenceSource.Type, result.Source);
        Assert.NotNull(result.RejectedCandidates);
        Assert.Contains(
            result.RejectedCandidates!,
            candidate => candidate.Generator == "internal short-code"
                && candidate.Confidence == InferenceConfidence.Low);
    }

    [Fact]
    public void Code_IsAcceptedOnlyUnderAggressive()
    {
        Assert.Equal(InferenceSource.Type, Resolve<Widget>("Code", SemanticInferenceMode.Conservative).Source);
        Assert.Equal(InferenceSource.Type, Resolve<Widget>("Code", SemanticInferenceMode.Balanced).Source);
        Assert.Equal(InferenceSource.Semantic, Resolve<Widget>("Code", SemanticInferenceMode.Aggressive).Source);
    }

    [Fact]
    public void HighConfidence_IsAcceptedInEveryMode()
    {
        foreach (SemanticInferenceMode mode in new[]
                 {
                     SemanticInferenceMode.Conservative,
                     SemanticInferenceMode.Balanced,
                     SemanticInferenceMode.Aggressive,
                 })
        {
            Assert.Equal(InferenceSource.Semantic, Resolve<Widget>("Email", mode).Source);
        }
    }

    [Fact]
    public void MediumConfidence_IsRejectedByConservativeAcceptedByBalanced()
    {
        // Widget.State -> Address.State (Medium).
        Assert.Equal(InferenceSource.Type, Resolve<Widget>("State", SemanticInferenceMode.Conservative).Source);
        Assert.Equal(InferenceSource.Semantic, Resolve<Widget>("State", SemanticInferenceMode.Balanced).Source);
    }

    [Fact]
    public void DisabledMode_SkipsSemanticEntirely()
    {
        ResolvedSource result = Resolve<Widget>("Email", SemanticInferenceMode.Disabled);
        Assert.Equal(InferenceSource.Type, result.Source);
    }
}
