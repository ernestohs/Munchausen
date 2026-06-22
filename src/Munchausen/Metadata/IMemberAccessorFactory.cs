namespace Munchausen.Metadata;

/// <summary>
/// Creates a <see cref="MemberAccessor"/> (compiled get/set delegates) for a
/// member. Isolated behind this seam so compiled-expression and reflection
/// strategies are interchangeable.
/// </summary>
internal interface IMemberAccessorFactory
{
    MemberAccessor CreateAccessor(MemberMetadata member);
}
