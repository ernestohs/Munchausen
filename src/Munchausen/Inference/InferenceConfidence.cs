namespace Munchausen.Inference;

/// <summary>
/// Confidence helpers over the public <see cref="InferenceConfidence"/> enum
/// (resolved through the enclosing <c>Munchausen</c> namespace).
/// </summary>
internal static class Confidence
{
    /// <summary>One confidence level below <paramref name="value"/>, never below Low.</summary>
    public static InferenceConfidence OneBelow(InferenceConfidence value) => value switch
    {
        InferenceConfidence.High => InferenceConfidence.Medium,
        InferenceConfidence.Medium => InferenceConfidence.Low,
        _ => InferenceConfidence.Low,
    };

    /// <summary>Higher rank = higher confidence, so "highest wins" is a numeric max.</summary>
    public static int Rank(InferenceConfidence value) => value switch
    {
        InferenceConfidence.High => 2,
        InferenceConfidence.Medium => 1,
        _ => 0,
    };

    /// <summary>Whether <paramref name="mode"/> accepts a semantic match at <paramref name="confidence"/>.</summary>
    public static bool AcceptedBy(SemanticInferenceMode mode, InferenceConfidence confidence) => mode switch
    {
        SemanticInferenceMode.Conservative => confidence == InferenceConfidence.High,
        SemanticInferenceMode.Balanced => confidence != InferenceConfidence.Low,
        SemanticInferenceMode.Aggressive => true,
        _ => false, // Disabled
    };
}
