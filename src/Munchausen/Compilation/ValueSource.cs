using Munchausen.Inference;

namespace Munchausen.Compilation;

/// <summary>How a member's value is produced at generation time.</summary>
internal abstract class ValueSource;

/// <summary>A constant boxed value from <c>With(expr, value)</c>.</summary>
internal sealed class ConstantSource(object? value) : ValueSource
{
    public object? Value { get; } = value;
}

/// <summary>
/// A value-producing delegate. Typed user delegates and semantic/type generators
/// are both adapted to this uniform shape at build time. Only user delegates have
/// their failures wrapped as <see cref="LieGenerationException"/>.
/// </summary>
internal sealed class DelegateSource(
    Func<GenerationContext, object?> generator, string description, bool isUserDelegate = false) : ValueSource
{
    public Func<GenerationContext, object?> Generator { get; } = generator;

    public string Description { get; } = description;

    public bool IsUserDelegate { get; } = isUserDelegate;
}

/// <summary>A nested object whose plan is found in the reachable-plan dictionary by type.</summary>
internal sealed class NestedSource(Type childType) : ValueSource
{
    public Type ChildType { get; } = childType;
}

/// <summary>A collection materialized from an element source over a size range.</summary>
internal sealed class CollectionSource(
    CollectionShape shape,
    Type elementType,
    ValueSource elementSource,
    CollectionSize size,
    Func<IReadOnlyList<object?>, object> materializer) : ValueSource
{
    public CollectionShape Shape { get; } = shape;

    public Type ElementType { get; } = elementType;

    public ValueSource ElementSource { get; } = elementSource;

    public CollectionSize Size { get; } = size;

    /// <summary>Builds the declared collection shape from generated elements (built at compile time).</summary>
    public Func<IReadOnlyList<object?>, object> Materializer { get; } = materializer;
}

/// <summary>Why a member is left untouched during population.</summary>
internal enum SkipReason
{
    Ignored,
    Preserved,
    Unsupported,
}

/// <summary>A member that is not populated (ignored, preserved, or unsupported).</summary>
internal sealed class SkippedSource(SkipReason reason) : ValueSource
{
    public SkipReason Reason { get; } = reason;
}
