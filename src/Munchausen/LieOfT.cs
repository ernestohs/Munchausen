using Munchausen.Compilation;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>
/// The zero-configuration automatic path: generates <typeparamref name="T"/> using
/// inference only. The compiled plan is cached per closed generic (and shared
/// process-wide); a compilation failure is cached and rethrown consistently.
/// </summary>
/// <typeparam name="T">The model type to generate.</typeparam>
public static class Lie<T>
{
    private static readonly Lazy<GenerationPlan> CachedPlan =
        new(() => DefinitionCompiler.Default.CompileAutomatic(typeof(T)),
            LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Generates a single inferred <typeparamref name="T"/>.</summary>
    public static T Generate(
        GenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var operation = new GenerationOperation(CachedPlan.Value, options, cancellationToken);
        return (T)operation.GenerateRoot(0);
    }

    /// <summary>Generates <paramref name="count"/> inferred <typeparamref name="T"/> instances.</summary>
    public static IReadOnlyList<T> Generate(
        int count,
        GenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        var operation = new GenerationOperation(CachedPlan.Value, options, cancellationToken);
        var results = new List<T>(count);
        for (long index = 0; index < count; index++)
        {
            results.Add((T)operation.GenerateRoot(index));
        }

        return results;
    }
}
