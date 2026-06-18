namespace Munchausen.Examples;

/// <summary>
/// The same type, definition, and seed produce identical output everywhere. The seed
/// can be set on the definition (<c>WithSeed</c>) or overridden per call
/// (<c>GenerationOptions.Seed</c>). Time-relative values come from a single reference
/// time, which you can pin directly or supply through a <see cref="TimeProvider"/>.
/// </summary>
internal sealed class DeterminismAndSeeding : ExampleChapter
{
    public string Title => "5. Determinism and seeding";

    public void Run()
    {
        // For byte-for-byte reproducibility of values that depend on time (dates),
        // pin the reference time as well as the seed. With only a seed, each call
        // captures the current time, so date members differ between runs.
        var anchor = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        LieDefinition<Customer> seeded = Lie.Define<Customer>()
            .WithDefaults(new GenerationDefaults { Seed = 42, ReferenceTime = anchor })
            .Build();

        IReadOnlyList<Customer> run1 = seeded.Generate(3);
        IReadOnlyList<Customer> run2 = seeded.Generate(3);
        bool identical = run1.Zip(run2).All(pair => Key(pair.First) == Key(pair.Second));
        Console.WriteLine($"Two runs at seed 42 (with pinned time) are identical: {identical}");
        Console.WriteLine($"  first customer both runs: {run1[0].FirstName} {run1[0].LastName}, {run1[0].CreatedAt:yyyy-MM-dd}");

        // A per-call seed overrides the definition's seed.
        Customer other = seeded.Generate(new GenerationOptions { Seed = 7 });
        Console.WriteLine($"  email at seed 7      : {other.Email} (differs from seed 42)");

        // Pin the reference time so date inference is fully determined.
        var fixedOptions = new GenerationOptions
        {
            Seed = 1,
            ReferenceTime = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };
        Customer pinned = Lie<Customer>.Generate(fixedOptions);
        Console.WriteLine($"{Environment.NewLine}ReferenceTime pinned to 2030-01-01:");
        Console.WriteLine($"  CreatedAt = {pinned.CreatedAt:yyyy-MM-dd} (a recent-past date before the reference)");

        // A TimeProvider is the testable seam for "now" when no ReferenceTime is given.
        var clock = new FixedClock(new DateTimeOffset(2020, 6, 15, 0, 0, 0, TimeSpan.Zero));
        Customer viaClock = Lie<Customer>.Generate(new GenerationOptions { Seed = 1, TimeProvider = clock });
        Console.WriteLine($"  via TimeProvider(2020-06-15): CreatedAt = {viaClock.CreatedAt:yyyy-MM-dd}");
    }

    private static string Key(Customer c) => $"{c.FirstName}|{c.LastName}|{c.Email}|{c.CreatedAt:O}";

    /// <summary>A fixed clock, shown to illustrate the <c>TimeProvider</c> seam without extra packages.</summary>
    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
