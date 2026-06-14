namespace Munchausen.Diagnostics;

/// <summary>One row of the diagnostic registry: code, default severity, title.</summary>
internal sealed record DiagnosticCodeInfo(string Code, LieDiagnosticSeverity DefaultSeverity, string Title);

/// <summary>
/// The diagnostic code registry, mirroring the documented list in API_DESIGN.md.
/// <c>DiagnosticRegistryConformanceTests</c> asserts this table matches the document.
/// </summary>
internal static class DiagnosticCodes
{
    public static IReadOnlyList<DiagnosticCodeInfo> All { get; } = new DiagnosticCodeInfo[]
    {
        new("LIE001", LieDiagnosticSeverity.Error, "Invalid member expression"),
        new("LIE002", LieDiagnosticSeverity.Error, "Conflicting member rules"),
        new("LIE003", LieDiagnosticSeverity.Error, "Unresolved required member"),
        new("LIE004", LieDiagnosticSeverity.Error, "Ambiguous constructor"),
        new("LIE005", LieDiagnosticSeverity.Error, "Invalid option"),
        new("LIE006", LieDiagnosticSeverity.Error, "Unknown locale"),
        new("LIE007", LieDiagnosticSeverity.Error, "Data-pack conflict"),
        new("LIE008", LieDiagnosticSeverity.Warning, "Unsupported constraint"),
        new("LIE009", LieDiagnosticSeverity.Info, "Self-type child definition"),
        new("LIE010", LieDiagnosticSeverity.Error, "Unique value exhaustion"),
        new("LIE011", LieDiagnosticSeverity.Warning, "Preserve target may never be assigned"),
    };

    public static LieDiagnosticSeverity SeverityOf(string code) =>
        All.First(entry => entry.Code == code).DefaultSeverity;
}
