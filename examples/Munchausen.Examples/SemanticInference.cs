namespace Munchausen.Examples;

/// <summary>
/// Semantic inference reads member names to pick a generator. The mode controls how
/// much confidence is required: Conservative (High only), Balanced (High + Medium,
/// the default), Aggressive (also Low), and Disabled (type inference only). The same
/// <see cref="Ticket"/> model is explained under each mode so the differences show.
/// </summary>
internal sealed class SemanticInference : ExampleChapter
{
    public string Title => "3. Semantic inference modes";

    public void Run()
    {
        SemanticInferenceMode[] modes =
        [
            SemanticInferenceMode.Conservative,
            SemanticInferenceMode.Balanced,
            SemanticInferenceMode.Aggressive,
            SemanticInferenceMode.Disabled,
        ];

        foreach (SemanticInferenceMode mode in modes)
        {
            InferenceReport report = Lie.Define<Ticket>()
                .WithDefaults(new GenerationDefaults { SemanticInference = mode })
                .Build()
                .Explain();

            Console.WriteLine($"{Environment.NewLine}Mode: {mode}");
            foreach (MemberInferenceReport m in report.Members)
            {
                string confidence = m.Confidence is { } c ? $" ({c})" : string.Empty;
                Console.WriteLine($"  {m.MemberPath,-6} -> {m.Generator,-18} [{m.Source}]{confidence}");
            }
        }
    }
}
