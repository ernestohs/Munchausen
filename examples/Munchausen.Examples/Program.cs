using Munchausen.Examples;

// Runs the example chapters in order, printing a labeled section for each.
// Pass a substring as the first argument to run only matching chapters, e.g.:
//   dotnet run --project examples/Munchausen.Examples -- datasets

ExampleChapter[] chapters =
[
    new BasicGeneration(),
    new BuilderCustomization(),
    new SemanticInference(),
    new Datasets(),
    new DeterminismAndSeeding(),
    new NestedCollectionsAndCycles(),
    new RecordsAndConstructors(),
    new ExplainAndIntrospection(),
    new ErrorHandling(),
    new CancellationAndContext(),
    new Recipes(),
];

List<ExampleChapter> selected = args.Length > 0
    ? chapters.Where(c => c.Title.Contains(args[0], StringComparison.OrdinalIgnoreCase)).ToList()
    : chapters.ToList();

if (selected.Count == 0)
{
    Console.WriteLine($"No chapter matched '{args[0]}'. Available chapters:");
    foreach (ExampleChapter chapter in chapters)
    {
        Console.WriteLine($"  - {chapter.Title}");
    }

    return;
}

foreach (ExampleChapter chapter in selected)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 72));
    Console.WriteLine($"  {chapter.Title}");
    Console.WriteLine(new string('=', 72));
    chapter.Run();
}
