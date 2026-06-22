namespace Munchausen.Examples;

/// <summary>
/// Every Generate call takes a <see cref="CancellationToken"/>; cancellation surfaces as
/// a plain <see cref="OperationCanceledException"/> (never wrapped). Inside a generator
/// delegate, the context exposes the root <c>Index</c> and the current <c>MemberPath</c>.
/// </summary>
internal sealed class CancellationAndContext : ExampleChapter
{
    public string Title => "10. Cancellation and generation context";

    public void Run()
    {
        // A cancelled token stops a large batch; the exception passes through unwrapped.
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        try
        {
            Lie<Customer>.Generate(1_000_000, new GenerationOptions { Seed = 1 }, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Batch canceled: OperationCanceledException passed through unwrapped.");
        }

        // context.Index is the zero-based position of the root object in the batch.
        IReadOnlyList<Customer> rows = Lie.Define<Customer>()
            .With(c => c.Email, data => $"user{data.Index}@example.com")
            .WithSeed(1)
            .Build()
            .Generate(3);
        Console.WriteLine($"{Environment.NewLine}Using context.Index per row:");
        foreach (Customer c in rows)
        {
            Console.WriteLine($"  {c.Email}");
        }

        // context.MemberPath names the member currently being generated.
        Customer one = Lie.Define<Customer>()
            .With(c => c.City, data => $"[{data.MemberPath}]")
            .WithSeed(1)
            .Build()
            .Generate();
        Console.WriteLine($"{Environment.NewLine}context.MemberPath for the City member: {one.City}");
    }
}
