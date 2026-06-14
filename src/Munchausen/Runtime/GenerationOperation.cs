using System.Reflection;
using Munchausen.Compilation;

namespace Munchausen.Runtime;

/// <summary>
/// One operation per public <c>Generate(...)</c> call. Owns the PRNG and reference
/// time (resolved once), tracks the traversal path, and executes the immutable
/// plan: construct, populate members in declaration order, run derivations in
/// registration order. Confined to the calling thread.
/// </summary>
internal sealed class GenerationOperation
{
    private const int DefaultMaximumDepth = 3;

    private readonly GenerationPlan _plan;
    private readonly CancellationToken _cancellation;
    private readonly int _maximumDepth;
    private readonly CycleBehavior _cycleBehavior;

    public GenerationOperation(GenerationPlan plan, GenerationOptions? options, CancellationToken cancellation)
    {
        _plan = plan;
        _cancellation = cancellation;

        int? seed = options?.Seed ?? plan.Defaults.Seed;
        Random = seed.HasValue
            ? new DeterministicRandom(seed.Value)
            // The single permitted Random.Shared use: entropy for an unseeded operation.
            : new DeterministicRandom(unchecked((ulong)System.Random.Shared.NextInt64()));

        ReferenceTime =
            options?.ReferenceTime
            ?? plan.Defaults.ReferenceTime
            ?? options?.TimeProvider?.GetUtcNow()
            ?? DateTimeOffset.UtcNow;

        _maximumDepth = plan.Defaults.MaximumDepth ?? DefaultMaximumDepth;
        _cycleBehavior = plan.Defaults.CycleBehavior ?? CycleBehavior.Terminate;

        Context = new GenerationContext(this);
    }

    public DeterministicRandom Random { get; }

    public DateTimeOffset ReferenceTime { get; }

    public GenerationContext Context { get; }

    public PathStack Path { get; } = new();

    public long Index { get; private set; }

    public object GenerateRoot(long index)
    {
        _cancellation.ThrowIfCancellationRequested();
        Index = index;
        Path.Reset();
        return GenerateObject(_plan.Root);
    }

    private object GenerateObject(TypePlan typePlan)
    {
        object instance = Construct(typePlan);
        Path.PushObject(typePlan.ModelType);
        try
        {
            foreach (MemberPlan member in typePlan.Members)
            {
                PopulateMember(typePlan.ModelType, instance, member);
            }

            _cancellation.ThrowIfCancellationRequested();
            foreach (DerivationPlan derivation in typePlan.Derivations)
            {
                RunDerivation(typePlan.ModelType, instance, derivation);
            }
        }
        finally
        {
            Path.PopObject();
        }

        return instance;
    }

    private object Construct(TypePlan typePlan)
    {
        switch (typePlan.Construction)
        {
            case UserDelegatePlan user:
                _cancellation.ThrowIfCancellationRequested();
                return InvokeUser(
                    () => user.Constructor(Context), typePlan.ModelType, GenerationPhase.Construction)!;

            case CompiledConstructorPlan compiled:
                var arguments = new object?[compiled.Parameters.Length];
                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = EvaluateSource(typePlan.ModelType, compiled.Parameters[i].Source);
                }

                return InvokeConstructor(compiled.Constructor, arguments, typePlan.ModelType);

            default:
                throw new InvalidOperationException($"No usable constructor for {typePlan.ModelType}.");
        }
    }

    private object InvokeConstructor(ConstructorInfo constructor, object?[] arguments, Type modelType)
    {
        try
        {
            return constructor.Invoke(arguments)!;
        }
        catch (TargetInvocationException invocation) when (invocation.InnerException is OperationCanceledException canceled)
        {
            throw canceled;
        }
        catch (TargetInvocationException invocation)
        {
            throw Wrap(invocation.InnerException ?? invocation, modelType, GenerationPhase.Construction);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw Wrap(exception, modelType, GenerationPhase.Construction);
        }
    }

    private void PopulateMember(Type modelType, object instance, MemberPlan member)
    {
        if (member.Source is SkippedSource)
        {
            return;
        }

        Path.PushMember(member.Member.Name);
        try
        {
            if (member.Source is NestedSource or CollectionSource)
            {
                _cancellation.ThrowIfCancellationRequested();
            }

            object? value = EvaluateSource(modelType, member.Source);
            member.Accessor.Setter?.Invoke(instance, value);
        }
        finally
        {
            Path.PopMember();
        }
    }

    private void RunDerivation(Type modelType, object instance, DerivationPlan derivation)
    {
        _cancellation.ThrowIfCancellationRequested();
        Path.PushMember(derivation.Member.Name);
        try
        {
            object? value = InvokeUser(
                () => derivation.Generator(Context, instance), modelType, GenerationPhase.Derivation);
            derivation.Accessor.Setter?.Invoke(instance, value);
        }
        finally
        {
            Path.PopMember();
        }
    }

    private object? EvaluateSource(Type modelType, ValueSource source)
    {
        switch (source)
        {
            case ConstantSource constant:
                return constant.Value;

            case DelegateSource { IsUserDelegate: true } user:
                return InvokeUser(() => user.Generator(Context), modelType, GenerationPhase.MemberPopulation);

            case DelegateSource delegateSource:
                return delegateSource.Generator(Context);

            case NestedSource nested:
                return GenerateNested(modelType, nested.ChildType);

            case CollectionSource collection:
                return MaterializeCollection(modelType, collection);

            default:
                return null;
        }
    }

    private object? GenerateNested(Type modelType, Type childType)
    {
        if (Path.Depth + 1 > _maximumDepth || Path.ContainsType(childType))
        {
            return TerminateOrThrow(modelType);
        }

        _cancellation.ThrowIfCancellationRequested();
        return GenerateObject(_plan.ReachablePlans[childType]);
    }

    private object MaterializeCollection(Type modelType, CollectionSource collection)
    {
        bool elementWouldTerminate =
            collection.ElementSource is SkippedSource
            || (collection.ElementSource is NestedSource nested
                && (Path.Depth + 1 > _maximumDepth || Path.ContainsType(nested.ChildType)));

        if (elementWouldTerminate)
        {
            if (_cycleBehavior == CycleBehavior.Throw)
            {
                throw DepthOrCycle(modelType);
            }

            return collection.Materializer(Array.Empty<object?>());
        }

        int size = Random.Int(collection.Size.MinCount, collection.Size.MaxCount);
        var elements = new object?[size];
        for (int i = 0; i < size; i++)
        {
            _cancellation.ThrowIfCancellationRequested();
            Path.PushMember($"[{i}]");
            try
            {
                elements[i] = EvaluateSource(modelType, collection.ElementSource);
            }
            finally
            {
                Path.PopMember();
            }
        }

        return collection.Materializer(elements);
    }

    private object? TerminateOrThrow(Type modelType) =>
        _cycleBehavior == CycleBehavior.Throw ? throw DepthOrCycle(modelType) : null;

    private LieGenerationException DepthOrCycle(Type modelType) => new(
        $"Generation of {modelType.Name} hit the maximum depth or a reference cycle at '{Path.ToMemberPath()}' (index {Index}).",
        modelType,
        Path.ToMemberPath(),
        Index,
        GenerationPhase.MemberPopulation,
        innerException: null);

    private object? InvokeUser(Func<object?> action, Type modelType, GenerationPhase phase)
    {
        try
        {
            return action();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (LieGenerationException)
        {
            throw; // already wrapped exactly once by a nested generation
        }
        catch (Exception exception)
        {
            throw Wrap(exception, modelType, phase);
        }
    }

    private LieGenerationException Wrap(Exception exception, Type modelType, GenerationPhase phase) => new(
        $"Generation of {modelType.Name} failed at '{Path.ToMemberPath()}' (index {Index}, phase {phase}).",
        modelType,
        Path.ToMemberPath(),
        Index,
        phase,
        exception);
}
