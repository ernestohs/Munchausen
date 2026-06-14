using Munchausen.Diagnostics;
using Munchausen.Inference;

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
                return new DelegateSource(Unbound, resolved.GeneratorName);

            case StructuralKind.Nested:
                Type childType = context.Structural.Type;
                if (childType == _declaringType)
                {
                    _diagnostics.SelfTypeChildDefinition(context.MemberName);
                }

                _enqueueReachable(childType);
                return new NestedSource(childType);

            case StructuralKind.Collection:
                Type elementType = context.Structural.ElementType ?? typeof(object);
                return new CollectionSource(
                    context.Structural.Shape,
                    elementType,
                    BuildElementSource(elementType),
                    CollectionSize.Between(1, 3));

            default:
                if (isRequired)
                {
                    _diagnostics.UnresolvedRequiredMember(context.MemberName);
                }

                return new SkippedSource(SkipReason.Unsupported);
        }
    }

    private ValueSource BuildElementSource(Type elementType)
    {
        StructuralClassification classification = StructuralClassifier.Classify(elementType);
        switch (classification.Kind)
        {
            case StructuralKind.Scalar:
                return new DelegateSource(Unbound, "element");

            case StructuralKind.Nested:
                _enqueueReachable(elementType);
                return new NestedSource(elementType);

            default:
                // Nested collections / unsupported elements are refined in M6.
                return new SkippedSource(SkipReason.Unsupported);
        }
    }
}
