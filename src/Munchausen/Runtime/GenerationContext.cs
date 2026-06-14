using Munchausen.Runtime;

namespace Munchausen;

/// <summary>
/// The façade passed to user rules during generation, exposing the operation's
/// randomness, datasets, reference time, and member context. A context is valid
/// only during the generation call that supplied it; capturing it and reading it
/// later observes the final state.
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

    /// <summary>Primitive random values and choices.</summary>
    public RandomData Random => Dataset<RandomData>();

    /// <summary>Dates relative to the operation's reference time.</summary>
    public DateData Date => Dataset<DateData>();

    /// <summary>Personal names.</summary>
    public NameData Name => Dataset<NameData>();

    /// <summary>Internet identifiers.</summary>
    public InternetData Internet => Dataset<InternetData>();

    /// <summary>Postal addresses and geography.</summary>
    public AddressData Address => Dataset<AddressData>();

    /// <summary>Lorem-style filler text.</summary>
    public LoremData Lorem => Dataset<LoremData>();

    /// <summary>Resolves a dataset by type, cached one instance per operation.</summary>
    public TDataset Dataset<TDataset>() => (TDataset)_operation.ResolveDataset(typeof(TDataset));
}
