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

    /// <summary>
    /// Sets the definition name, surfaced on the built definition and in diagnostics.
    /// An empty or whitespace name causes <see cref="Build"/> to emit LIE005.
    /// </summary>
    public LieDefinitionBuilder<T> WithName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _state.Name = name;
        return this;
    }

    /// <summary>
    /// Assigns a constant value to the targeted member. An invalid member expression
    /// causes <see cref="Build"/> to emit LIE001; combining this with
    /// <see cref="Ignore{TProperty}"/> or <see cref="Preserve{TProperty}"/> on the
    /// same member emits LIE002.
    /// </summary>
    public LieDefinitionBuilder<T> With<TProperty>(
        Expression<Func<T, TProperty>> property,
        TProperty value)
    {
        ArgumentNullException.ThrowIfNull(property);
        _state.AddMemberRule(property, MemberRuleKind.WithValue, value);
        return this;
    }

    /// <summary>
    /// Assigns the targeted member from a generator delegate. An invalid member
    /// expression causes <see cref="Build"/> to emit LIE001; a conflicting rule on
    /// the same member emits LIE002.
    /// </summary>
    public LieDefinitionBuilder<T> With<TProperty>(
        Expression<Func<T, TProperty>> property,
        Func<GenerationContext, TProperty> generator)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(generator);
        _state.AddMemberRule(property, MemberRuleKind.WithGenerator, generator);
        return this;
    }

    /// <summary>
    /// Derives the targeted member from the partially populated instance, after
    /// member population. An invalid member expression causes <see cref="Build"/> to
    /// emit LIE001; combining a derivation with any other rule on the same member
    /// emits LIE002.
    /// </summary>
    public LieDefinitionBuilder<T> Derive<TProperty>(
        Expression<Func<T, TProperty>> property,
        Func<GenerationContext, T, TProperty> generator)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(generator);
        _state.AddMemberRule(property, MemberRuleKind.Derive, generator);
        return this;
    }

    /// <summary>
    /// Unlike <see cref="Preserve{TProperty}"/>, which keeps an existing
    /// constructor or initializer value, <c>Ignore</c> leaves the targeted member at
    /// its type default and does not populate it. An invalid member expression
    /// causes <see cref="Build"/> to emit LIE001; combining <c>Ignore</c> with a
    /// <c>With</c> or <c>Preserve</c> on the same member emits LIE002.
    /// </summary>
    public LieDefinitionBuilder<T> Ignore<TProperty>(Expression<Func<T, TProperty>> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        _state.AddMemberRule(property, MemberRuleKind.Ignore, payload: null);
        return this;
    }

    /// <summary>
    /// Unlike <see cref="Ignore{TProperty}"/>, which leaves the member at its type
    /// default, <c>Preserve</c> keeps whatever value the targeted member already holds
    /// after construction or initialization. An invalid member expression causes
    /// <see cref="Build"/> to emit LIE001; combining <c>Preserve</c> with a <c>With</c>
    /// or <c>Ignore</c> on the same member emits LIE002.
    /// </summary>
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
    public LieDefinition<T> Build() =>
        new(Compilation.DefinitionCompiler.Default.Compile(typeof(T), _state));
}
