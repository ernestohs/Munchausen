namespace Munchausen.Examples;

/// <summary>
/// Task-oriented recipes that combine the building blocks into the things you
/// actually reach for: test fixtures, coherent object graphs, reusable child
/// definitions, reproducible cohorts, replay-by-seed, inference contracts, and
/// bulk generation.
/// </summary>
internal sealed class Recipes : ExampleChapter
{
    private static readonly DateTimeOffset Anchor = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public string Title => "11. Real-world recipes";

    public void Run()
    {
        TestFixture();
        Console.WriteLine();
        CoherentPerson();
        Console.WriteLine();
        CoherentInvoice();
        Console.WriteLine();
        ReproduceBySeed();
        Console.WriteLine();
        DatedCohort();
        Console.WriteLine();
        ExplainAsContract();
        Console.WriteLine();
        Bulk();
    }

    // Arrange a believable subject, pinning only the fields the test asserts on.
    private static void TestFixture()
    {
        Console.WriteLine("-- Test fixture: realistic, with the asserted fields pinned --");

        Customer subject = Lie.Define<Customer>()
            .With(c => c.Email, "known@example.com")
            .With(c => c.Age, 30)
            .WithSeed(1)
            .Build()
            .Generate();

        Console.WriteLine($"   {subject.FirstName} {subject.LastName}, {subject.Email}, age {subject.Age}, {subject.City}");
    }

    // Derive one member from others so the instance is internally consistent.
    private static void CoherentPerson()
    {
        Console.WriteLine("-- Coherent data: the email is derived from the generated name --");

        Customer person = Lie.Define<Customer>()
            .With(c => c.FirstName, d => d.Name.First())
            .With(c => c.LastName, d => d.Name.Last())
            .Derive(c => c.Email, (d, c) => d.Internet.Email(c.FirstName, c.LastName))
            .WithSeed(2)
            .Build()
            .Generate();

        Console.WriteLine($"   {person.FirstName} {person.LastName} -> {person.Email}");
    }

    // Compose a reusable child definition into a parent, then derive coherent totals.
    private static void CoherentInvoice()
    {
        Console.WriteLine("-- A composed object graph with coherent, derived money fields --");

        LieDefinition<LineItem> line = Lie.Define<LineItem>()
            .With(l => l.Description, d => d.Dataset<CommerceData>().ProductName())
            .With(l => l.Quantity, d => d.Random.Int(1, 5))
            .With(l => l.UnitPrice, d => d.Dataset<CommerceData>().Price(5m, 200m))
            .Derive(l => l.LineTotal, (d, l) => l.Quantity * l.UnitPrice)
            .Build();

        Invoice invoice = Lie.Define<Invoice>()
            .With(i => i.Number, d => $"INV-{d.Random.Int(1000, 9999)}")
            .With(i => i.Lines, d => d.GenerateMany(line, 3))           // reuse the child definition
            .Derive(i => i.Subtotal, (d, i) => i.Lines.Sum(l => l.LineTotal))
            .Derive(i => i.Tax, (d, i) => Math.Round(i.Subtotal * 0.10m, 2))
            .Derive(i => i.Total, (d, i) => i.Subtotal + i.Tax)         // sees Subtotal and Tax (earlier derivations)
            .Derive(i => i.DueOn, (d, i) => i.IssuedOn.AddDays(30))
            .WithDefaults(new GenerationDefaults { Seed = 3, ReferenceTime = Anchor })
            .Build()
            .Generate();

        Console.WriteLine($"   {invoice.Number} for {invoice.BillTo.FirstName} {invoice.BillTo.LastName}");
        foreach (LineItem l in invoice.Lines)
        {
            Console.WriteLine($"     {l.Quantity} x {l.Description} @ {l.UnitPrice} = {l.LineTotal}");
        }

        Console.WriteLine($"   Subtotal {invoice.Subtotal}, Tax {invoice.Tax}, Total {invoice.Total}");
        Console.WriteLine($"   Issued {invoice.IssuedOn:yyyy-MM-dd}, due {invoice.DueOn:yyyy-MM-dd}");
        Console.WriteLine($"   subtotal == sum(lines): {invoice.Subtotal == invoice.Lines.Sum(l => l.LineTotal)}");
    }

    // Commit the seed of an interesting case to reproduce it exactly later.
    private static void ReproduceBySeed()
    {
        Console.WriteLine("-- Reproduce an exact case by committing its seed --");

        const int seed = 1234;
        var options = new GenerationOptions { Seed = seed, ReferenceTime = Anchor };
        Customer first = Lie<Customer>.Generate(options);
        Customer replay = Lie<Customer>.Generate(options);

        Console.WriteLine($"   seed {seed}: {first.Email} == {replay.Email} -> {first.Email == replay.Email}");
    }

    // Generate a deterministic, dated population by pinning the seed and reference time.
    private static void DatedCohort()
    {
        Console.WriteLine("-- A dated cohort: deterministic signups across the last year --");

        IReadOnlyList<Customer> cohort = Lie.Define<Customer>()
            .With(c => c.CreatedAt, d => d.Date.Past(years: 1))
            .WithDefaults(new GenerationDefaults { Seed = 7, ReferenceTime = Anchor })
            .Build()
            .Generate(5);

        foreach (Customer c in cohort.OrderBy(c => c.CreatedAt))
        {
            Console.WriteLine($"   {c.CreatedAt:yyyy-MM-dd}  {c.FirstName} {c.LastName}");
        }
    }

    // Use Explain to lock an inference decision, exactly as a contract test would.
    private static void ExplainAsContract()
    {
        Console.WriteLine("-- Lock an inference decision with Explain --");

        InferenceReport report = Lie.Define<Car>().Build().Explain();
        MemberInferenceReport make = report.Members.Single(m => m.MemberPath == "Make");
        bool asExpected = make.Source == InferenceSource.Semantic && make.Generator == "Vehicle.Make";

        Console.WriteLine($"   Car.Make resolves via {make.Source}/{make.Generator} ({make.Confidence})");
        Console.WriteLine($"   matches expectation: {asExpected}");
    }

    // Generating large batches stays fast because all reflection happens at Build time.
    private static void Bulk()
    {
        Console.WriteLine("-- Bulk generation for load and performance scenarios --");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        IReadOnlyList<Customer> rows = Lie<Customer>.Generate(100_000, new GenerationOptions { Seed = 1 });
        stopwatch.Stop();

        Console.WriteLine($"   generated {rows.Count:N0} customers in {stopwatch.ElapsedMilliseconds} ms");
    }
}
