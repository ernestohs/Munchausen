namespace Munchausen;

/// <summary>Entry point for defining mock-data generators.</summary>
public static class Lie
{
    /// <summary>Begins a new definition for model type <typeparamref name="T"/>.</summary>
    public static LieDefinitionBuilder<T> Define<T>() => new();
}
