using System.Collections.Immutable;
using System.Reflection;
using Munchausen.Diagnostics;
using Munchausen.Inference;
using Munchausen.Metadata;

namespace Munchausen.Compilation;

/// <summary>A constructor parameter and the value source that fills it.</summary>
internal sealed class ParameterPlan(Type parameterType, string name, ValueSource source)
{
    public Type ParameterType { get; } = parameterType;

    public string Name { get; } = name;

    public ValueSource Source { get; } = source;
}

/// <summary>Construct by invoking a selected public constructor.</summary>
internal sealed class CompiledConstructorPlan(ConstructorInfo constructor, ImmutableArray<ParameterPlan> parameters)
    : ConstructionPlan
{
    public ConstructorInfo Constructor { get; } = constructor;

    public ImmutableArray<ParameterPlan> Parameters { get; } = parameters;
}

/// <summary>Construct with a user <c>ConstructWith</c> delegate.</summary>
internal sealed class UserDelegatePlan(object constructor) : ConstructionPlan
{
    public object Constructor { get; } = constructor;
}

/// <summary>No usable construction; emitted alongside a diagnostic so compilation can continue.</summary>
internal sealed class NoConstructionPlan : ConstructionPlan;

/// <summary>
/// Selects a constructor per the documented order: a single <c>[LieConstructor]</c>,
/// otherwise the most resolvable, preferring parameterless only on a tie, else LIE004.
/// Parameter resolvability reuses the inference pipeline (a dry run per parameter).
/// </summary>
internal sealed class ConstructorPlanner(InferenceEngine engine)
{
    public ConstructionPlan Plan(
        Type type,
        ModelMetadata metadata,
        object? constructWith,
        SemanticInferenceMode mode,
        MemberSourceBuilder sources,
        DiagnosticBag diagnostics)
    {
        if (constructWith is not null)
        {
            return new UserDelegatePlan(constructWith);
        }

        IReadOnlyList<ConstructorMetadata> constructors = metadata.Constructors;

        var marked = constructors
            .Where(c => c.Constructor.IsDefined(typeof(LieConstructorAttribute), inherit: false))
            .ToList();
        if (marked.Count == 1)
        {
            return Compile(type, marked[0], mode, sources);
        }

        if (marked.Count > 1)
        {
            diagnostics.AmbiguousConstructor(
                $"{type.Name} has multiple constructors marked [LieConstructor]");
            return Compile(type, marked[0], mode, sources);
        }

        if (constructors.Count == 0)
        {
            diagnostics.AmbiguousConstructor($"{type.Name} has no public constructor");
            return new NoConstructionPlan();
        }

        var scored = constructors
            .Select(c => (Constructor: c, Score: ResolvableCount(type, c, mode)))
            .ToList();
        int best = scored.Max(s => s.Score);
        var winners = scored.Where(s => s.Score == best).ToList();

        if (winners.Count == 1)
        {
            return Compile(type, winners[0].Constructor, mode, sources);
        }

        ConstructorMetadata? parameterless = winners
            .Find(s => s.Constructor.Parameters.Count == 0).Constructor;
        if (parameterless is not null)
        {
            return Compile(type, parameterless, mode, sources);
        }

        diagnostics.AmbiguousConstructor(
            $"{type.Name} has multiple equally resolvable constructors and no deterministic winner");
        return Compile(type, winners[0].Constructor, mode, sources);
    }

    private int ResolvableCount(Type type, ConstructorMetadata constructor, SemanticInferenceMode mode)
    {
        int count = 0;
        foreach (ParameterMetadata parameter in constructor.Parameters)
        {
            var context = new MemberInferenceContext(
                type, parameter.Name, parameter.ValueType, $"{type.Name}.ctor({parameter.Name})", mode);
            ResolvedSource resolved = engine.Resolve(context);
            if (resolved.Source != InferenceSource.Unsupported)
            {
                count++;
            }
        }

        return count;
    }

    private static CompiledConstructorPlan Compile(
        Type type, ConstructorMetadata constructor, SemanticInferenceMode mode, MemberSourceBuilder sources)
    {
        ImmutableArray<ParameterPlan> parameters = constructor.Parameters
            .Select(parameter =>
            {
                var context = new MemberInferenceContext(
                    type, parameter.Name, parameter.ValueType, $"{type.Name}.ctor({parameter.Name})", mode);
                ValueSource source = sources.BuildInferred(context, isRequired: false, out _);
                return new ParameterPlan(parameter.ValueType, parameter.Name, source);
            })
            .ToImmutableArray();

        return new CompiledConstructorPlan(constructor.Constructor, parameters);
    }
}
