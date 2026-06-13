namespace Munchausen.Metadata;

/// <summary>
/// Get/set delegates for a single member. A null delegate means the operation is
/// unavailable (for example, no getter, or a read-only property has no setter).
/// </summary>
internal sealed class MemberAccessor
{
    public MemberAccessor(Func<object, object?>? getter, Action<object, object?>? setter)
    {
        Getter = getter;
        Setter = setter;
    }

    public Func<object, object?>? Getter { get; }

    public Action<object, object?>? Setter { get; }
}
