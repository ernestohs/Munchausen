using Munchausen;
using Munchausen.Diagnostics;
using Xunit;

namespace Munchausen.Tests;

/// <summary>
/// Independent transcription of the diagnostic code list in API_DESIGN.md,
/// compared against the implemented registry. Makes the code list executable.
/// </summary>
public sealed class DiagnosticRegistryConformanceTests
{
    private static readonly (string Code, LieDiagnosticSeverity Severity, string Title)[] Expected =
    {
        ("LIE001", LieDiagnosticSeverity.Error, "Invalid member expression"),
        ("LIE002", LieDiagnosticSeverity.Error, "Conflicting member rules"),
        ("LIE003", LieDiagnosticSeverity.Error, "Unresolved required member"),
        ("LIE004", LieDiagnosticSeverity.Error, "Ambiguous constructor"),
        ("LIE005", LieDiagnosticSeverity.Error, "Invalid option"),
        ("LIE006", LieDiagnosticSeverity.Error, "Unknown locale"),
        ("LIE007", LieDiagnosticSeverity.Error, "Data-pack conflict"),
        ("LIE008", LieDiagnosticSeverity.Warning, "Unsupported constraint"),
        ("LIE009", LieDiagnosticSeverity.Info, "Self-type child definition"),
        ("LIE010", LieDiagnosticSeverity.Error, "Unique value exhaustion"),
        ("LIE011", LieDiagnosticSeverity.Warning, "Preserve target may never be assigned"),
    };

    [Fact]
    public void Registry_MatchesDocumentedCodesAndSeverities()
    {
        Assert.Equal(Expected.Length, DiagnosticCodes.All.Count);

        for (int i = 0; i < Expected.Length; i++)
        {
            (string code, LieDiagnosticSeverity severity, string title) = Expected[i];
            DiagnosticCodeInfo actual = DiagnosticCodes.All[i];

            Assert.Equal(code, actual.Code);
            Assert.Equal(severity, actual.DefaultSeverity);
            Assert.Equal(title, actual.Title);
        }
    }
}
