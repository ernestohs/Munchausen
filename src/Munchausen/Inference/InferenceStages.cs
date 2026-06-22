using Munchausen.Metadata;

namespace Munchausen.Inference;

/// <summary>
/// Per-member context handed to each inference stage. Built from a property's
/// <see cref="MemberMetadata"/> or, for a constructor parameter, from its raw
/// name and type, so parameters and properties share one inference pipeline.
/// </summary>
internal sealed class MemberInferenceContext
{
    public MemberInferenceContext(
        Type modelType, string memberName, Type valueType, string memberPath, SemanticInferenceMode semanticMode)
    {
        ModelType = modelType;
        MemberName = memberName;
        ValueType = valueType;
        MemberPath = memberPath;
        SemanticMode = semanticMode;
        Structural = StructuralClassifier.Classify(valueType);
    }

    public MemberInferenceContext(
        Type modelType, MemberMetadata member, string memberPath, SemanticInferenceMode semanticMode)
        : this(modelType, member.Name, member.ValueType, memberPath, semanticMode)
    {
        Member = member;
    }

    public Type ModelType { get; }

    public string MemberName { get; }

    public Type ValueType { get; }

    /// <summary>The full metadata when the context describes a property; null for a constructor parameter.</summary>
    public MemberMetadata? Member { get; }

    public string MemberPath { get; }

    public SemanticInferenceMode SemanticMode { get; }

    public StructuralClassification Structural { get; }

    /// <summary>Candidates considered but not selected, accumulated across stages for Explain.</summary>
    public List<RejectedCandidate> RejectedCandidates { get; } = new();
}

/// <summary>A stage in the build-time inference pipeline.</summary>
internal interface IInferenceStage
{
    bool TryResolve(MemberInferenceContext context, out ResolvedSource source);
}

/// <summary>Stage 5: name-and-model semantic matching, filtered by the selected mode.</summary>
internal sealed class SemanticStage : IInferenceStage
{
    public bool TryResolve(MemberInferenceContext context, out ResolvedSource source)
    {
        source = null!;
        if (context.Structural.Kind != StructuralKind.Scalar
            || context.SemanticMode == SemanticInferenceMode.Disabled)
        {
            return false;
        }

        IReadOnlyList<ScoredCandidate> scored = SemanticMatcher.Match(
            context.MemberName, context.ModelType.Name, context.ValueType);
        if (scored.Count == 0)
        {
            return false;
        }

        // Highest confidence wins; ties resolve to the earliest catalog row.
        ScoredCandidate best = scored[0];
        foreach (ScoredCandidate candidate in scored)
        {
            if (Confidence.Rank(candidate.Confidence) > Confidence.Rank(best.Confidence))
            {
                best = candidate;
            }
        }

        if (Confidence.AcceptedBy(context.SemanticMode, best.Confidence))
        {
            foreach (ScoredCandidate candidate in scored)
            {
                if (!ReferenceEquals(candidate, best))
                {
                    context.RejectedCandidates.Add(new RejectedCandidate(
                        candidate.Generator, candidate.Confidence, "lower-confidence candidate"));
                }
            }

            source = ResolvedSource.Semantic(best.Generator, best.Confidence);
            return true;
        }

        foreach (ScoredCandidate candidate in scored)
        {
            context.RejectedCandidates.Add(new RejectedCandidate(
                candidate.Generator, candidate.Confidence, $"rejected by {context.SemanticMode} mode"));
        }

        return false;
    }
}

/// <summary>Stage 6: type-based defaults for any supported scalar.</summary>
internal sealed class TypeStage : IInferenceStage
{
    public bool TryResolve(MemberInferenceContext context, out ResolvedSource source)
    {
        source = null!;
        if (context.Structural.Kind != StructuralKind.Scalar)
        {
            return false;
        }

        string? description = TypeDefaults.DescribeFor(context.Structural);
        if (description is null)
        {
            return false;
        }

        source = ResolvedSource.Type(description);
        return true;
    }
}

/// <summary>Stage 7: marks anything unresolved as unsupported.</summary>
internal sealed class FallbackStage : IInferenceStage
{
    public bool TryResolve(MemberInferenceContext context, out ResolvedSource source)
    {
        source = ResolvedSource.Unsupported(context.Structural);
        return true;
    }
}
