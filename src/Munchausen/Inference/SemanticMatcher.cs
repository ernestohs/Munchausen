namespace Munchausen.Inference;

/// <summary>
/// Scores the semantic catalog against a single member per INFERENCE_CATALOG.md
/// matching rules 3-5 (value-type gate, name match, confidence assignment).
/// Mode filtering is applied by <see cref="SemanticStage"/>, not here.
/// </summary>
internal static class SemanticMatcher
{
    public static IReadOnlyList<ScoredCandidate> Match(string memberName, string modelTypeName, Type valueType)
    {
        string member = MemberNameNormalizer.Normalize(memberName);
        string model = MemberNameNormalizer.Normalize(modelTypeName);
        Type effectiveType = Nullable.GetUnderlyingType(valueType) ?? valueType;

        var results = new List<ScoredCandidate>();
        foreach (SemanticCandidate candidate in SemanticCatalog.Entries)
        {
            if (Array.IndexOf(candidate.ValueTypes, effectiveType) < 0)
            {
                continue;
            }

            string? exactName = Array.Find(candidate.Names, name => name == member);
            string? suffixName = exactName is null
                ? Array.Find(candidate.Names, name => member.Length > name.Length && member.EndsWith(name, StringComparison.Ordinal))
                : null;

            if (exactName is null && suffixName is null)
            {
                continue;
            }

            (InferenceConfidence confidence, string generator) =
                ComputeExactResult(candidate, model, exactName ?? suffixName!);

            // Rule 4 final bullet: a suffix match is one level below the exact-match result.
            if (exactName is null)
            {
                confidence = Confidence.OneBelow(confidence);
            }

            results.Add(new ScoredCandidate(candidate, confidence, generator, exactName is not null));
        }

        return results;
    }

    private static (InferenceConfidence Confidence, string Generator) ComputeExactResult(
        SemanticCandidate candidate, string normalizedModel, string matchedName)
    {
        InferenceConfidence baseConfidence = candidate.BaseConfidence;
        if (candidate.ReducedConfidenceNames is { } reduced && Array.IndexOf(reduced, matchedName) >= 0)
        {
            baseConfidence = Confidence.OneBelow(baseConfidence);
        }

        if (candidate.Hints is null)
        {
            return (baseConfidence, candidate.Generator);
        }

        bool hintMatched = Array.Exists(
            candidate.Hints, hint => normalizedModel.Contains(hint, StringComparison.Ordinal));
        if (hintMatched)
        {
            // Rule 4 bullet 1: an exact name match with a matching hint is High.
            return (InferenceConfidence.High, candidate.Generator);
        }

        // Hint miss: per-row override (Option 1) if present, else rule 4 bullet 3.
        return (
            candidate.NoHintConfidence ?? Confidence.OneBelow(candidate.BaseConfidence),
            candidate.NoHintGenerator ?? candidate.Generator);
    }
}
