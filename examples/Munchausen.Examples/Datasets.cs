namespace Munchausen.Examples;

/// <summary>
/// The datasets reachable from <c>GenerationContext</c> inside a generator delegate:
/// Name, Internet, Address, Date, Lorem, and Random are first-class properties;
/// Vehicle and Commerce are resolved with <c>Dataset&lt;T&gt;()</c>. All of them are
/// seeded, so the values below are reproducible.
/// </summary>
internal sealed class Datasets : ExampleChapter
{
    public string Title => "4. Datasets (Name, Internet, Address, Date, Lorem, Random, Vehicle, Commerce)";

    public void Run()
    {
        Profile p = Lie.Define<Profile>()
            .With(x => x.FullName, d => d.Name.FullName())
            .With(x => x.Email, d => d.Internet.Email())
            .With(x => x.UserName, d => d.Internet.UserName())
            .With(x => x.Website, d => d.Internet.Url())
            .With(x => x.IpAddress, d => d.Internet.IpAddress())
            .With(x => x.Street, d => d.Address.StreetAddress())
            .With(x => x.City, d => d.Address.City())
            .With(x => x.Latitude, d => d.Address.Latitude())
            .With(x => x.Bio, d => d.Lorem.Sentence())
            .With(x => x.MemberSince, d => d.Date.Past(years: 3))
            .With(x => x.Roll, d => d.Random.Int(1, 6))
            .With(x => x.Balance, d => d.Random.Decimal(0m, 1000m))
            .With(x => x.IsActive, d => d.Random.Bool(0.8))
            .With(x => x.Tier, d => d.Random.Pick("Bronze", "Silver", "Gold"))
            .With(x => x.Tags, d => d.Random.Sample(["new", "vip", "trial", "partner"], 2))
            .With(x => x.LastStatus, d => d.Random.Enum<OrderStatus>())
            .With(x => x.CarMake, d => d.Dataset<VehicleData>().Make())
            .With(x => x.Vin, d => d.Dataset<VehicleData>().Vin())
            .With(x => x.FeaturedProduct, d => d.Dataset<CommerceData>().ProductName())
            .With(x => x.Sku, d => d.Dataset<CommerceData>().Sku())
            .With(x => x.Currency, d => d.Dataset<CommerceData>().CurrencyCode())
            .WithSeed(2024)
            .Build()
            .Generate();

        Console.WriteLine($"Name.FullName        : {p.FullName}");
        Console.WriteLine($"Internet.Email       : {p.Email}");
        Console.WriteLine($"Internet.UserName    : {p.UserName}");
        Console.WriteLine($"Internet.Url         : {p.Website}");
        Console.WriteLine($"Internet.IpAddress   : {p.IpAddress}");
        Console.WriteLine($"Address.StreetAddress: {p.Street}");
        Console.WriteLine($"Address.City         : {p.City}");
        Console.WriteLine($"Address.Latitude     : {p.Latitude}");
        Console.WriteLine($"Lorem.Sentence       : {p.Bio}");
        Console.WriteLine($"Date.Past(3)         : {p.MemberSince:yyyy-MM-dd}");
        Console.WriteLine($"Random.Int(1,6)      : {p.Roll}");
        Console.WriteLine($"Random.Decimal       : {p.Balance}");
        Console.WriteLine($"Random.Bool(0.8)     : {p.IsActive}");
        Console.WriteLine($"Random.Pick          : {p.Tier}");
        Console.WriteLine($"Random.Sample(2)     : {string.Join(", ", p.Tags)}");
        Console.WriteLine($"Random.Enum          : {p.LastStatus}");
        Console.WriteLine($"Vehicle.Make         : {p.CarMake}");
        Console.WriteLine($"Vehicle.Vin          : {p.Vin}");
        Console.WriteLine($"Commerce.ProductName : {p.FeaturedProduct}");
        Console.WriteLine($"Commerce.Sku         : {p.Sku}");
        Console.WriteLine($"Commerce.CurrencyCode: {p.Currency}");

        // Random.Weighted draws values in proportion to their weights.
        IReadOnlyList<Customer> plans = Lie.Define<Customer>()
            .With(c => c.City, d => d.Random.Weighted(
            [
                new WeightedValue<string>("Free", 0.70),
                new WeightedValue<string>("Pro", 0.25),
                new WeightedValue<string>("Enterprise", 0.05),
            ]))
            .WithSeed(7)
            .Build()
            .Generate(1000);

        Console.WriteLine($"{Environment.NewLine}Random.Weighted over 1000 draws:");
        foreach (IGrouping<string, Customer> g in plans.GroupBy(c => c.City).OrderByDescending(g => g.Count()))
        {
            Console.WriteLine($"  {g.Key,-11}: {g.Count()}");
        }
    }
}
