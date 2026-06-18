namespace Munchausen.Examples;

/// <summary>
/// One self-contained section of the examples tour. Each chapter writes its own
/// labeled output to the console when <see cref="Run"/> is called.
/// </summary>
internal interface ExampleChapter
{
    /// <summary>Short title shown as the section header.</summary>
    string Title { get; }

    /// <summary>Runs the chapter, printing its demonstration to the console.</summary>
    void Run();
}
