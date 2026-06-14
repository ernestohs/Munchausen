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

    /// <summary>Generates a nested <typeparamref name="TModel"/> using automatic inference.</summary>
    public TModel Generate<TModel>() => (TModel)_operation.GenerateChild(typeof(TModel), explicitPlan: null)!;

    /// <summary>Generates <paramref name="count"/> nested <typeparamref name="TModel"/> instances.</summary>
    public IReadOnlyList<TModel> GenerateMany<TModel>(int count) => GenerateMany<TModel>(count, plan: null);

    /// <summary>Generates a nested <typeparamref name="TModel"/> using the supplied definition.</summary>
    public TModel Generate<TModel>(LieDefinition<TModel> definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        return (TModel)_operation.GenerateChild(typeof(TModel), definition.Plan)!;
    }

    /// <summary>Generates <paramref name="count"/> nested <typeparamref name="TModel"/> using the supplied definition.</summary>
    public IReadOnlyList<TModel> GenerateMany<TModel>(LieDefinition<TModel> definition, int count)
    {
        ArgumentNullException.ThrowIfNull(definition);
        return GenerateMany<TModel>(count, definition.Plan);
    }

    private IReadOnlyList<TModel> GenerateMany<TModel>(int count, Munchausen.Compilation.GenerationPlan? plan)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        var results = new List<TModel>(count);
        for (int i = 0; i < count; i++)
        {
            results.Add((TModel)_operation.GenerateChild(typeof(TModel), plan)!);
        }

        return results;
    }
}

