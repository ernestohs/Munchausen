using System.Text;

namespace Munchausen;

/// <summary>
/// A structured explanation of how a definition resolves every member, produced
/// by <see cref="LieDefinition{T}.Explain"/>. <see cref="ToText"/> renders a
/// human-readable form whose exact prose is not contractual.
/// </summary>
public sealed class InferenceReport
{
    internal InferenceReport(
        string? definitionName,
        Type modelType,
        string locale,
        bool isComplete,
        IReadOnlyList<MemberInferenceReport> members,
        IReadOnlyList<LieDiagnostic> diagnostics)
    {
        DefinitionName = definitionName;
        ModelType = modelType;
        Locale = locale;
        IsComplete = isComplete;
        Members = members;
        Diagnostics = diagnostics;
    }

    /// <summary>The optional definition name.</summary>
    public string? DefinitionName { get; }

    /// <summary>The model type explained.</summary>
    public Type ModelType { get; }

    /// <summary>The active locale.</summary>
    public string Locale { get; }

    /// <summary>True when every member resolves to a usable source.</summary>
    public bool IsComplete { get; }

    /// <summary>One entry per member; nested members appear as a single entry.</summary>
    public IReadOnlyList<MemberInferenceReport> Members { get; }

    /// <summary>Info/Warning diagnostics retained from the build.</summary>
    public IReadOnlyList<LieDiagnostic> Diagnostics { get; }

    /// <summary>Renders a human-readable report. Format is not contractual.</summary>
    public string ToText()
    {
        var builder = new StringBuilder();
        builder.Append(ModelType.Name);
        if (DefinitionName is not null)
        {
            builder.Append(" \"").Append(DefinitionName).Append('"');
        }

        builder.Append(IsComplete ? " (complete)" : " (incomplete)").Append('\n');

        foreach (MemberInferenceReport member in Members)
        {
            builder.Append("  ").Append(ModelType.Name).Append('.').Append(member.MemberPath)
                .Append(" -> ").Append(member.Generator)
                .Append(" [").Append(Reason(member.Source)).Append(']');
            if (member.Confidence is { } confidence)
            {
                builder.Append(' ').Append(confidence);
            }

            builder.Append('\n');
        }

        foreach (LieDiagnostic diagnostic in Diagnostics)
        {
            builder.Append("  ").Append(diagnostic.Code).Append(": ").Append(diagnostic.Message).Append('\n');
        }

        return builder.ToString();
    }

    private static string Reason(InferenceSource source) => source switch
    {
        InferenceSource.Semantic => "property name + model",
        InferenceSource.Type => "type",
        InferenceSource.Explicit => "explicit",
        InferenceSource.Derived => "derived",
        InferenceSource.Ignored => "ignored",
        InferenceSource.Preserved => "preserved",
        InferenceSource.ChildDefinition => "nested object",
        InferenceSource.Unsupported => "unsupported",
        _ => source.ToString(),
    };
}
