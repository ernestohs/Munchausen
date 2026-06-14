using Munchausen.Compilation;

namespace Munchausen;

/// <summary>
/// An immutable, reusable, thread-safe compiled definition. Produced by
/// <see cref="LieDefinitionBuilder{T}.Build"/>; generation and explanation are
/// implemented in later milestones.
/// </summary>
/// <typeparam name="T">The model type this definition generates.</typeparam>
public sealed class LieDefinition<T>
{
    private readonly GenerationPlan _plan;

    internal LieDefinition(GenerationPlan plan)
    {
        _plan = plan;
        Name = plan.DefinitionName;
    }

    internal GenerationPlan Plan => _plan;

    /// <summary>The optional definition name set through <see cref="LieDefinitionBuilder{T}.WithName"/>.</summary>
    public string? Name { get; }

    /// <summary>Generates a single <typeparamref name="T"/>.</summary>
    public T Generate(
        GenerationOptions? options = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <summary>Generates <paramref name="count"/> instances of <typeparamref name="T"/>.</summary>
    public IReadOnlyList<T> Generate(
        int count,
        GenerationOptions? options = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <summary>Returns a structured explanation of how each member is resolved.</summary>
    public InferenceReport Explain() => throw new NotImplementedException();
}
