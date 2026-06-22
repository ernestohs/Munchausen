namespace Munchausen.Examples;

/// <summary>
/// <c>Explain()</c> inspects the compiled plan without generating values. It reports
/// each member's source, generator, and confidence, which rules were overridden, and
/// whether the plan is complete. Use the structured <see cref="InferenceReport"/> for
/// stable assertions; <c>ToText()</c> is for humans and its wording is not contractual.
/// </summary>
internal sealed class ExplainAndIntrospection : ExampleChapter
{
    public string Title => "8. Explain and introspection";

    public void Run()
    {
        // The second With on Make overrides the first; the report records that.
        InferenceReport report = Lie.Define<Car>()
            .WithName("Showroom cars")
            .With(c => c.Make, "Tesla")
            .With(c => c.Make, "Toyota")
            .Build()
            .Explain();

        Console.WriteLine("Human-readable ToText():");
        Console.WriteLine(Indent(report.ToText()));

        Console.WriteLine($"Definition : {report.DefinitionName}");
        Console.WriteLine($"Model      : {report.ModelType.Name}");
        Console.WriteLine($"Complete   : {report.IsComplete}");
        Console.WriteLine("Members (structured):");
        foreach (MemberInferenceReport m in report.Members)
        {
            string confidence = m.Confidence is { } c ? $" {c}" : string.Empty;
            string overridden = m.OverriddenRules.Count > 0
                ? $"  (overrode: {string.Join(", ", m.OverriddenRules)})"
                : string.Empty;
            Console.WriteLine($"  {m.MemberPath,-6} {m.Source,-9} {m.Generator}{confidence}{overridden}");
        }
    }

    private static string Indent(string text) =>
        string.Join(Environment.NewLine, text.Split(Environment.NewLine).Select(line => "  " + line));
}
