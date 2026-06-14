namespace Munchausen.Inference;

/// <summary>
/// Runs structural classification, then the scalar stage pipeline. The full
/// documented order is: ExplicitRule (handled by the compiler in M5), the empty
/// v1.1 provider/attribute slots, Semantic, Type, Fallback. v1.0 implements the
/// last three plus the structural routing.
/// </summary>
internal sealed class InferenceEngine
{
    private readonly IInferenceStage[] _scalarStages =
    {
        new SemanticStage(),
        new TypeStage(),
        new FallbackStage(),
    };

    public ResolvedSource Resolve(MemberInferenceContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        switch (context.Structural.Kind)
        {
            case StructuralKind.Nested:
                return Attach(context, ResolvedSource.Nested(context.Structural));

            case StructuralKind.Collection:
                return Attach(context, ResolvedSource.Collection(context.Structural));

            case StructuralKind.Unsupported:
                return Attach(context, ResolvedSource.Unsupported(context.Structural));

            default:
                foreach (IInferenceStage stage in _scalarStages)
                {
                    if (stage.TryResolve(context, out ResolvedSource source))
                    {
                        return Attach(context, source);
                    }
                }

                // Unreachable: FallbackStage always resolves.
                return Attach(context, ResolvedSource.Unsupported(context.Structural));
        }
    }

    private static ResolvedSource Attach(MemberInferenceContext context, ResolvedSource source) =>
        context.RejectedCandidates.Count == 0
            ? source
            : source with { RejectedCandidates = context.RejectedCandidates.ToArray() };
}
