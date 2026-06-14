using System.Linq.Expressions;
using Munchausen.Builder;

namespace Munchausen;

/// <summary>
/// Configures a definition for model type <typeparamref name="T"/>. Mutable and
/// not thread-safe; methods append rules and return <c>this</c>. Nothing is
/// parsed or validated until <see cref="Build"/>.
/// </summary>
/// <typeparam name="T">The model type being configured.</typeparam>
public sealed class LieDefinitionBuilder<T>
{
    private readonly BuilderState _state = new();

    // Builders are created only through Lie.Define<T>(); the v1.0 surface has no public ctor.
    internal LieDefinitionBuilder()
    {
    }

    internal BuilderState State => _state;

    /// <summary>Sets the definition name, surfaced on the built definition and in diagnostics.</summary>
    public LieDefinitionBuilder<T> WithName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _state.Name = name;
        return this;
    }

    /// <summary>Assigns a constant value to the targeted member.</summary>
    public LieDefinitionBuilder<T> With<TProperty>(
        Expression<Func<T, TProperty>> property,
        TProperty value)
    {
        ArgumentNullException.ThrowIfNull(property);
        _state.AddMemberRule(property, MemberRuleKind.WithValue, value);
        return this;
    }

    /// <summary>Assigns the targeted member from a generator delegate.</summary>
    public LieDefinitionBuilder<T> With<TProperty>(
        Expression<Func<T, TProperty>> property,
        Func<GenerationContext, TProperty> generator)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(generator);
        _state.AddMemberRule(property, MemberRuleKind.WithGenerator, generator);
        return this;
    }

    /// <summary>Derives the targeted member from the partially populated instance.</summary>
    public LieDefinitionBuilder<T> Derive<TProperty>(
        Expression<Func<T, TProperty>> property,
        Func<GenerationContext, T, TProperty> generator)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(generator);
        _state.AddMemberRule(property, MemberRuleKind.Derive, generator);
        return this;
    }

    /// <summary>Leaves the targeted member at its type default (not populated).</summary>
    public LieDefinitionBuilder<T> Ignore<TProperty>(Expression<Func<T, TProperty>> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        _state.AddMemberRule(property, MemberRuleKind.Ignore, payload: null);
        return this;
    }

    /// <summary>Preserves whatever value the targeted member already holds after construction.</summary>
    public LieDefinitionBuilder<T> Preserve<TProperty>(Expression<Func<T, TProperty>> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        _state.AddMemberRule(property, MemberRuleKind.Preserve, payload: null);
        return this;
    }

    /// <summary>Constructs the model with a custom delegate instead of an inferred constructor.</summary>
    public LieDefinitionBuilder<T> ConstructWith(Func<GenerationContext, T> constructor)
    {
        ArgumentNullException.ThrowIfNull(constructor);
        _state.Construction = new ConstructionRecord(constructor);
        return this;
    }

    /// <summary>Sets the default seed. Equivalent to <c>WithDefaults(new GenerationDefaults { Seed = seed })</c>.</summary>
    public LieDefinitionBuilder<T> WithSeed(int seed) =>
        WithDefaults(new GenerationDefaults { Seed = seed });

    /// <summary>Merges defaults into the accumulated defaults with per-property last-write-wins.</summary>
    public LieDefinitionBuilder<T> WithDefaults(GenerationDefaults defaults)
    {
        ArgumentNullException.ThrowIfNull(defaults);
        _state.MergeDefaults(defaults);
        return this;
    }

    /// <summary>Compiles the accumulated configuration into an immutable definition.</summary>
    public LieDefinition<T> Build() => throw new NotImplementedException();
}
