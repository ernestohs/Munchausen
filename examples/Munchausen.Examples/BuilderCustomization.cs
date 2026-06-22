namespace Munchausen.Examples;

/// <summary>
/// The fluent builder: pin members with <c>With</c>, compute them with <c>Derive</c>,
/// skip them with <c>Ignore</c>, and keep initializer values with <c>Preserve</c>.
/// Anything left unconfigured is still inferred.
/// </summary>
internal sealed class BuilderCustomization : ExampleChapter
{
    public string Title => "2. Builder customization (With / Derive / Ignore / Preserve)";

    public void Run()
    {
        LieDefinition<Customer> vip = Lie.Define<Customer>()
            .WithName("VIP customers")
            .With(c => c.Country, "United States")                  // a constant value
            .With(c => c.FirstName, data => data.Name.First())      // a generator delegate
            .Derive(c => c.Email, (data, c) =>                      // computed from the built object
                $"{c.FirstName}.{c.LastName}@vip.example.com".ToLowerInvariant())
            .Ignore(c => c.Age)                                     // left at the type default (0)
            .WithSeed(42)
            .Build();

        // The compiled definition is immutable and reusable.
        Customer a = vip.Generate();
        Console.WriteLine($"Definition name    : {vip.Name}");
        Console.WriteLine($"Country (constant) : {a.Country}");
        Console.WriteLine($"FirstName (gen)    : {a.FirstName}");
        Console.WriteLine($"LastName (inferred): {a.LastName}");
        Console.WriteLine($"Email (derived)    : {a.Email}");
        Console.WriteLine($"Age (ignored)      : {a.Age}");

        // Preserve keeps a constructor or initializer value instead of inferring it.
        Coupon coupon = Lie.Define<Coupon>()
            .Preserve(x => x.IsActive)     // IsActive keeps its initializer (true)
            .WithSeed(1)
            .Build()
            .Generate();

        Console.WriteLine($"{Environment.NewLine}Coupon.Code      : {coupon.Code}");
        Console.WriteLine($"Coupon.IsActive  : {coupon.IsActive} (preserved initializer)");
    }
}
