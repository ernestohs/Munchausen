namespace Munchausen.Examples;

/// <summary>
/// Nested objects and collections fill in automatically to a bounded depth. Cycles
/// are handled by <see cref="CycleBehavior"/>: Terminate (the default) yields null or
/// empty at a cycle or the depth limit, while Throw raises a generation exception.
/// </summary>
internal sealed class NestedCollectionsAndCycles : ExampleChapter
{
    public string Title => "6. Nested objects, collections, and cycles";

    public void Run()
    {
        // Order has a nested Customer, a collection of Items, and two Address members.
        Order order = Lie<Order>.Generate(new GenerationOptions { Seed = 3 });
        Console.WriteLine("Automatic nesting of an Order:");
        Console.WriteLine($"  Customer       : {order.Customer.FirstName} {order.Customer.LastName}");
        Console.WriteLine($"  Items          : {order.Items.Count} generated");
        Console.WriteLine($"  BillingAddress : {order.BillingAddress.City}");
        Console.WriteLine($"  ShippingAddress: {order.ShippingAddress.City} (a sibling, not a cycle)");

        // Employee.Manager is self-referential; Terminate sets it to null.
        Employee emp = Lie<Employee>.Generate(new GenerationOptions { Seed = 5 });
        Console.WriteLine($"{Environment.NewLine}Self-reference under Terminate (default):");
        Console.WriteLine($"  {emp.Name}, Manager is null: {emp.Manager is null}");

        // Throw surfaces the cycle as a LieGenerationException instead.
        try
        {
            Lie.Define<Employee>()
                .WithDefaults(new GenerationDefaults { CycleBehavior = CycleBehavior.Throw })
                .WithSeed(5)
                .Build()
                .Generate();
            Console.WriteLine("  (no exception under Throw - unexpected)");
        }
        catch (LieGenerationException ex)
        {
            Console.WriteLine($"  Throw mode raised: {ex.Phase} at \"{ex.MemberPath}\"");
        }

        // MaximumDepth caps how far a chain of distinct types is followed.
        Level1 shallow = Lie.Define<Level1>()
            .WithDefaults(new GenerationDefaults { MaximumDepth = 1 })
            .WithSeed(1)
            .Build()
            .Generate();
        Level1 deep = Lie<Level1>.Generate(new GenerationOptions { Seed = 1 });
        Console.WriteLine($"{Environment.NewLine}Depth limit (Level1 -> Level2 -> Level3):");
        Console.WriteLine($"  MaximumDepth=1 : Next set={shallow.Next is not null}, Next.Next set={shallow.Next?.Next is not null}");
        Console.WriteLine($"  default depth  : Next set={deep.Next is not null}, Next.Next set={deep.Next?.Next is not null}");
    }
}
