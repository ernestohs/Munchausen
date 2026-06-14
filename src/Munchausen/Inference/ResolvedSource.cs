namespace Munchausen.Inference;

/// <summary>
/// The inference pipeline's per-member result. The compiler (M5) translates this
/// into a plan <c>ValueSource</c> + report entry. The exact public InferenceSource
/// mapping for nested/collection members is finalized with the report family (M8).
/// </summary>
internal sealed record ResolvedSource(
    InferenceSource Source,
    InferenceConfidence? Confidence,
    string GeneratorName,
    StructuralClassification? Structural = null,
    IReadOnlyList<RejectedCandidate>? RejectedCandidates = null)
{
    public static ResolvedSource Semantic(string generator, InferenceConfidence confidence) =>
        new(InferenceSource.Semantic, confidence, generator);

    public static ResolvedSource Type(string generator) =>
        new(InferenceSource.Type, Confidence: null, generator);

    public static ResolvedSource Nested(StructuralClassification structural) =>
        new(InferenceSource.Type, Confidence: null, "(nested object)", structural);

    public static ResolvedSource Collection(StructuralClassification structural) =>
        new(InferenceSource.Type, Confidence: null, "(collection)", structural);

    public static ResolvedSource Unsupported(StructuralClassification? structural = null) =>
        new(InferenceSource.Unsupported, Confidence: null, "(unsupported)", structural);
}
