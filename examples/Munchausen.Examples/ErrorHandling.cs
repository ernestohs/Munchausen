namespace Munchausen.Examples;

/// <summary>
/// Build-time problems are aggregated into a single <see cref="LieDefinitionException"/>
/// carrying every diagnostic with its stable LIE code. Failures inside a user delegate
/// at generation time are wrapped once as a <see cref="LieGenerationException"/> that
/// names the model, member, index, and phase, and keeps the original as InnerException.
/// </summary>
internal sealed class ErrorHandling : ExampleChapter
{
    public string Title => "9. Error handling (LIE codes and generation failures)";

    public void Run()
    {
        // Build() reports all detectable errors at once, not just the first.
        try
        {
            Lie.Define<Customer>()
                .WithName("   ")                 // LIE005: invalid (blank) name
                .With(c => c.City.Length, 5)     // LIE001: not a simple x => x.Member expression
                .With(c => c.Age, 30)
                .Ignore(c => c.Age)              // LIE002: conflicts with the With above
                .Build();
        }
        catch (LieDefinitionException ex)
        {
            Console.WriteLine($"Build failed: {ex.Message}");
            foreach (LieDiagnostic d in ex.Diagnostics)
            {
                Console.WriteLine($"  {d.Code} [{d.Severity}] {d.Message}");
            }
        }

        // A throwing generator delegate becomes a LieGenerationException at generation.
        LieDefinition<Customer> faulty = Lie.Define<Customer>()
            .With(c => c.Email, _ => throw new InvalidOperationException("upstream service is down"))
            .WithSeed(1)
            .Build();

        try
        {
            faulty.Generate();
        }
        catch (LieGenerationException ex)
        {
            Console.WriteLine($"{Environment.NewLine}Generation failed:");
            Console.WriteLine($"  {ex.ModelType.Name}.{ex.MemberPath} at index {ex.GenerationIndex}, phase {ex.Phase}");
            Console.WriteLine($"  inner: {ex.InnerException?.Message}");
        }

        Console.WriteLine($"{Environment.NewLine}(OperationCanceledException is never wrapped - see chapter 10.)");
    }
}
