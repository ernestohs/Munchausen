namespace Munchausen.Inference;

/// <summary>
/// One row of the semantic candidate table. Names and hints are pre-normalized.
/// The optional fields capture per-row behavior that overrides the generic
/// matching rule #4: <see cref="NoHintConfidence"/>/<see cref="NoHintGenerator"/>
/// for hint-gated rows (e.g. Vehicle make/model/year), and
/// <see cref="ReducedConfidenceNames"/> for names that match one level below base
/// (e.g. bare <c>address</c>).
/// </summary>
internal sealed record SemanticCandidate(
    string[] Names,
    string[]? Hints,
    Type[] ValueTypes,
    InferenceConfidence BaseConfidence,
    string Generator,
    InferenceConfidence? NoHintConfidence = null,
    string? NoHintGenerator = null,
    string[]? ReducedConfidenceNames = null);

/// <summary>A candidate scored against a specific member, after rules 3-4.</summary>
internal sealed record ScoredCandidate(
    SemanticCandidate Candidate,
    InferenceConfidence Confidence,
    string Generator,
    bool IsExact);

/// <summary>A candidate that did not win, retained for <c>Explain()</c>.</summary>
internal sealed record RejectedCandidate(
    string Generator,
    InferenceConfidence Confidence,
    string Reason);
