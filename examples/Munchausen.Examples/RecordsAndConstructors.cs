namespace Munchausen.Examples;

/// <summary>
/// Records and constructor-based types work out of the box: constructor parameters use
/// the same inference as properties. <see cref="LieConstructorAttribute"/> picks a
/// constructor when there is more than one, and <c>ConstructWith</c> takes over
/// construction entirely, using the context to generate nested values on demand.
/// </summary>
internal sealed class RecordsAndConstructors : ExampleChapter
{
    public string Title => "7. Records and constructors";

    public void Run()
    {
        // A record is generated through its primary constructor.
        Product product = Lie<Product>.Generate(new GenerationOptions { Seed = 11 });
        Console.WriteLine("Record via its constructor:");
        Console.WriteLine($"  {product.Name} [{product.Sku}] {product.Price} {product.Currency} ({product.Category})");

        // [LieConstructor] selects the marked constructor over the parameterless one.
        Reservation reservation = Lie<Reservation>.Generate(new GenerationOptions { Seed = 12 });
        Console.WriteLine($"{Environment.NewLine}Constructor chosen by [LieConstructor]:");
        Console.WriteLine($"  Code={reservation.Code}, CheckIn={reservation.CheckIn:yyyy-MM-dd}, GuestName={reservation.GuestName}");

        // ConstructWith builds the object itself, generating nested values via the context.
        Order order = Lie.Define<Order>()
            .ConstructWith(data => new Order(
                data.Generate<Customer>(),       // one nested Customer
                data.GenerateMany<Item>(3),      // exactly three Items
                OrderStatus.Paid))
            .WithSeed(99)
            .Build()
            .Generate();

        Console.WriteLine($"{Environment.NewLine}ConstructWith with nested generation:");
        Console.WriteLine($"  Order for {order.Customer.FirstName}, {order.Items.Count} items, status {order.Status}");
    }
}
