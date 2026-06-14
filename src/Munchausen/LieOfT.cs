namespace Munchausen;

/// <summary>
/// The zero-configuration automatic path: generates <typeparamref name="T"/> using
/// inference only, with a process-wide cached plan.
/// </summary>
/// <typeparam name="T">The model type to generate.</typeparam>
public static class Lie<T>
{
    /// <summary>Generates a single inferred <typeparamref name="T"/>.</summary>
    public static T Generate(
        GenerationOptions? options = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <summary>Generates <paramref name="count"/> inferred <typeparamref name="T"/> instances.</summary>
    public static IReadOnlyList<T> Generate(
        int count,
        GenerationOptions? options = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}
