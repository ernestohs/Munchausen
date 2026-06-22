namespace Munchausen;

/// <summary>
/// A single structured diagnostic produced by compilation or reported by
/// <c>Explain()</c>. Codes are stable; messages may improve over versions.
/// </summary>
/// <param name="Code">The stable diagnostic code, e.g. <c>LIE002</c>.</param>
/// <param name="Severity">The diagnostic severity.</param>
/// <param name="Message">A human-readable message.</param>
/// <param name="ModelType">The model type the diagnostic concerns, if any.</param>
/// <param name="MemberPath">The member path the diagnostic concerns, if any.</param>
public sealed record LieDiagnostic(
    string Code,
    LieDiagnosticSeverity Severity,
    string Message,
    Type? ModelType = null,
    string? MemberPath = null);
