namespace Munchausen.Runtime;

/// <summary>
/// Tracks the active member path and the chain of object frames for one root
/// traversal. Depth is the object-frame count (collections do not push a frame);
/// cycle detection is path-based, so repeated sibling types are allowed.
/// </summary>
internal sealed class PathStack
{
    private readonly List<string> _segments = new();
    private readonly List<Type> _objectTypes = new();

    /// <summary>Object depth: 0 at the root, +1 per nested object frame.</summary>
    public int Depth => _objectTypes.Count - 1;

    public void PushObject(Type type) => _objectTypes.Add(type);

    public void PopObject() => _objectTypes.RemoveAt(_objectTypes.Count - 1);

    public bool ContainsType(Type type) => _objectTypes.Contains(type);

    public void PushMember(string name) => _segments.Add(name);

    public void PopMember() => _segments.RemoveAt(_segments.Count - 1);

    public string ToMemberPath() => string.Join(".", _segments);

    public void Reset()
    {
        _segments.Clear();
        _objectTypes.Clear();
    }
}
