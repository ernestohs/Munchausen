using Munchausen.Runtime;

namespace Munchausen;

/// <summary>
/// The façade passed to user rules during generation, exposing the operation's
/// randomness, datasets, reference time, and services. Dataset members bind in
/// later milestones. A context is valid only during the generation call that
/// supplied it; capturing it and reading it later observes the final state.
/// </summary>
public sealed class GenerationContext
{
    private readonly GenerationOperation _operation;

    internal GenerationContext(GenerationOperation operation) => _operation = operation;

    /// <summary>The zero-based index of the root object currently being generated.</summary>
    public long Index => _operation.Index;

    /// <summary>The active locale. Always <c>en</c> in v1.0.</summary>
    public string Locale => "en";

    /// <summary>The member path currently being generated, e.g. <c>Owner.Email</c>.</summary>
    public string MemberPath => _operation.Path.ToMemberPath();

    internal Munchausen.Runtime.DeterministicRandom Random => _operation.Random;

    internal DateTimeOffset ReferenceTime => _operation.ReferenceTime;
}
