using Munchausen.Diagnostics;
using Munchausen.Inference;
using Munchausen.Runtime;

namespace Munchausen.Compilation;

/// <summary>
/// Turns an inference result into a plan <see cref="ValueSource"/>, discovering
/// reachable child types and emitting structural diagnostics (self-type LIE009,
/// unresolved-required LIE003). Inferred scalar generators are placeholders until
/// the dataset generators bind in M7; generation is stubbed until M6, so they are
/// never invoked before then.
/// </summary>
internal sealed class MemberSourceBuilder
{
    private static readonly Func<GenerationContext, object?> Unbound =
        _ => throw new NotImplementedException("Inferred generators are bound in M7.");

    private readonly InferenceEngine _engine;
    private readonly DiagnosticBag _diagnostics;
    private readonly Type _declaringType;
    private readonly Action<Type> _enqueueReachable;

    public MemberSourceBuilder(
        InferenceEngine engine, DiagnosticBag diagnostics, Type declaringType, Action<Type> enqueueReachable)
    {
        _engine = engine;
        _diagnostics = diagnostics;
        _declaringType = declaringType;
        _enqueueReachable = enqueueReachable;
    }

    public ValueSource BuildInferred(MemberInferenceContext context, bool isRequired, out ResolvedSource resolved)
    {
        resolved = _engine.Resolve(context);

        switch (context.Structural.Kind)
        {
            case StructuralKind.Scalar:
                return BuildScalarSource(context, resolved);

            case StructuralKind.Nested:
                Type childType = context.Structural.Type;
                if (childType == _declaringType)
                {
                    _diagnostics.SelfTypeChildDefinition(context.MemberName);
                }

                _enqueueReachable(childType);
                return new NestedSource(childType);

            case StructuralKind.Collection:
                return BuildCollectionSource(context.Structural);

            default:
                if (isRequired)
                {
                    _diagnostics.UnresolvedRequiredMember(context.MemberName);
                }

                return new SkippedSource(SkipReason.Unsupported);
        }
    }

    private static ValueSource BuildScalarSource(MemberInferenceContext context, ResolvedSource resolved)
    {
        // Semantic generators bind in M7; only pure type defaults bind now.
        if (resolved.Source == InferenceSource.Type)
        {
            Type type = Nullable.GetUnderlyingType(context.ValueType) ?? context.ValueType;
            Func<GenerationContext, object?>? generator = TypeDefaultGenerators.For(type);
            if (generator is not null)
            {
                return new DelegateSource(generator, resolved.GeneratorName);
            }
        }

        return new DelegateSource(Unbound, resolved.GeneratorName);
    }

    private CollectionSource BuildCollectionSource(StructuralClassification structural)
    {
        Func<IReadOnlyList<object?>, object> materializer = structural.Shape switch
        {
            CollectionShape.Array => CollectionMaterializers.ForArray(structural.ElementType!),
            CollectionShape.Dictionary or CollectionShape.IDictionary or CollectionShape.IReadOnlyDictionary =>
                CollectionMaterializers.ForEmptyDictionary(structural.KeyType!, structural.ValueType!),
            _ => CollectionMaterializers.ForList(structural.ElementType!),
        };

        Type elementType = structural.ElementType ?? typeof(object);
        return new CollectionSource(
            structural.Shape,
            elementType,
            BuildElementSource(elementType),
            CollectionSize.Between(1, 3),
            materializer);
    }

    private ValueSource BuildElementSource(Type elementType)
    {
        StructuralClassification classification = StructuralClassifier.Classify(elementType);
        switch (classification.Kind)
        {
            case StructuralKind.Scalar:
                Type type = Nullable.GetUnderlyingType(elementType) ?? elementType;
                Func<GenerationContext, object?>? generator = TypeDefaultGenerators.For(type);
                return new DelegateSource(generator ?? Unbound, "element");

            case StructuralKind.Nested:
                _enqueueReachable(elementType);
                return new NestedSource(elementType);

            default:
                // Nested collections / unsupported elements are refined in M6.
                return new SkippedSource(SkipReason.Unsupported);
        }
    }
}
