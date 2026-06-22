namespace Munchausen.Examples;

/// <summary>
/// The automatic path: <c>Lie&lt;T&gt;.Generate(...)</c> infers every member from the
/// type with no configuration. A seed is passed only so the printed output is stable.
/// </summary>
internal sealed class BasicGeneration : ExampleChapter
{
    public string Title => "1. Basic generation (zero configuration)";

    public void Run()
    {
        // One instance. Every member is inferred from its name and type.
        Customer one = Lie<Customer>.Generate(new GenerationOptions { Seed = 7 });

        Console.WriteLine("A single Customer:");
        Console.WriteLine($"  Id        = {one.Id}");
        Console.WriteLine($"  FirstName = {one.FirstName}");
        Console.WriteLine($"  LastName  = {one.LastName}");
        Console.WriteLine($"  Email     = {one.Email}");
        Console.WriteLine($"  Age       = {one.Age}");
        Console.WriteLine($"  City      = {one.City}");
        Console.WriteLine($"  Country   = {one.Country}");
        Console.WriteLine($"  CreatedAt = {one.CreatedAt:yyyy-MM-dd}");

        // Pass a count for a batch.
        IReadOnlyList<Customer> many = Lie<Customer>.Generate(3, new GenerationOptions { Seed = 7 });

        Console.WriteLine($"{Environment.NewLine}A batch of {many.Count}:");
        foreach (Customer c in many)
        {
            Console.WriteLine($"  {c.FirstName} {c.LastName} <{c.Email}>, age {c.Age}, {c.City}");
        }
    }
}
