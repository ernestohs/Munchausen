namespace Munchausen.Diagnostics;

/// <summary>
/// Build-time diagnostic accumulator. Helpers centralize message formatting so
/// code/severity pairs cannot drift from the registry.
/// </summary>
internal sealed class DiagnosticBag
{
    private readonly List<LieDiagnostic> _diagnostics = new();
    private readonly Type _modelType;

    public DiagnosticBag(Type modelType) => _modelType = modelType;

    public IReadOnlyList<LieDiagnostic> Diagnostics => _diagnostics;

    public bool HasErrors => _diagnostics.Exists(d => d.Severity == LieDiagnosticSeverity.Error);

    public void Add(string code, string message, string? memberPath = null) =>
        _diagnostics.Add(new LieDiagnostic(
            code, DiagnosticCodes.SeverityOf(code), message, _modelType, memberPath));

    public void InvalidMemberExpression(string message) =>
        Add("LIE001", message);

    public void ConflictingRules(string memberName) =>
        Add("LIE002", $"Conflicting member rules for {_modelType.Name}.{memberName}", memberName);

    public void UnresolvedRequiredMember(string memberName) =>
        Add("LIE003", $"Required member {_modelType.Name}.{memberName} could not be resolved", memberName);

    public void AmbiguousConstructor(string message) =>
        Add("LIE004", message);

    public void InvalidOption(string message) =>
        Add("LIE005", message);

    public void SelfTypeChildDefinition(string memberName) =>
        Add("LIE009", $"Member {_modelType.Name}.{memberName} references its own type", memberName);
}
