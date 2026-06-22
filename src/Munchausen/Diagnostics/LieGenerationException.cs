namespace Munchausen;

/// <summary>
/// Thrown when generation fails. User-code and runtime failures are wrapped
/// exactly once and preserved as <see cref="Exception.InnerException"/>;
/// <see cref="OperationCanceledException"/> is never wrapped.
/// </summary>
public sealed class LieGenerationException : LieException
{
    internal LieGenerationException(
        string message,
        Type modelType,
        string memberPath,
        long generationIndex,
        GenerationPhase phase,
        Exception? innerException)
        : base(message, innerException)
    {
        ModelType = modelType;
        MemberPath = memberPath;
        GenerationIndex = generationIndex;
        Phase = phase;
    }

    /// <summary>The model type being generated when the failure occurred.</summary>
    public Type ModelType { get; }

    /// <summary>The member path being generated when the failure occurred.</summary>
    public string MemberPath { get; }

    /// <summary>The zero-based root index being generated when the failure occurred.</summary>
    public long GenerationIndex { get; }

    /// <summary>The lifecycle phase during which the failure occurred.</summary>
    public GenerationPhase Phase { get; }
}
