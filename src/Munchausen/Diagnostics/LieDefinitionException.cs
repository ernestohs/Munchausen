namespace Munchausen;

/// <summary>
/// Thrown by <see cref="LieDefinitionBuilder{T}.Build"/> when a definition is
/// invalid. Carries every detectable error as structured <see cref="Diagnostics"/>;
/// the <see cref="Exception.Message"/> summarizes the first error and the count of
/// the rest, so naive logging is already actionable.
/// </summary>
public sealed class LieDefinitionException : Exception
{
    internal LieDefinitionException(string message, IReadOnlyList<LieDiagnostic> diagnostics)
        : base(message)
    {
        Diagnostics = diagnostics;
    }

    /// <summary>Every diagnostic produced by the failed build, in production order.</summary>
    public IReadOnlyList<LieDiagnostic> Diagnostics { get; }

    internal static LieDefinitionException FromDiagnostics(
        Type modelType, IReadOnlyList<LieDiagnostic> diagnostics)
    {
        var errors = diagnostics.Where(d => d.Severity == LieDiagnosticSeverity.Error).ToList();
        LieDiagnostic first = errors[0];

        string message = $"Definition for {modelType.Name} is invalid: {first.Message} ({first.Code}).";
        int remaining = errors.Count - 1;
        if (remaining > 0)
        {
            message += $" ({remaining} more error{(remaining == 1 ? string.Empty : "s")}; see Diagnostics.)";
        }

        return new LieDefinitionException(message, diagnostics);
    }
}
