namespace Munchausen;

/// <summary>Controls what happens when generation reaches a reference cycle or the maximum depth.</summary>
public enum CycleBehavior
{
    /// <summary>Stop descending: null references, empty collections, untouched optional members.</summary>
    Terminate,

    /// <summary>Throw a generation exception at the offending path.</summary>
    Throw,
}
