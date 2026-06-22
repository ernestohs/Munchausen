namespace Munchausen;

/// <summary>Severity of a <see cref="LieDiagnostic"/>.</summary>
public enum LieDiagnosticSeverity
{
    /// <summary>Informational; does not prevent a successful build.</summary>
    Info,

    /// <summary>A warning; does not prevent a successful build by itself.</summary>
    Warning,

    /// <summary>An error; <see cref="LieDefinitionBuilder{T}.Build"/> fails when any are present.</summary>
    Error,
}
