namespace Munchausen.Tests.Fixtures;

/// <summary>Pure-PRNG members (bound in M6); used for reproducibility and goldens.</summary>
public sealed class Primitives
{
    public int Number { get; set; }
    public bool Flag { get; set; }
    public Guid Id { get; set; }
    public double Ratio { get; set; }
    public long Big { get; set; }
}

/// <summary>Lifecycle ordering probe: construction, population, derivation.</summary>
public sealed class LifecycleModel
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }
    public int E { get; set; }
}

/// <summary>Directly self-referential — terminates by cycle at depth 1.</summary>
public sealed class Node
{
    public Guid Id { get; set; }
    public Node? Next { get; set; }
}

public sealed class Leaf
{
    public int Value { get; set; }
}

/// <summary>Two members of the same nested type are siblings, not a cycle.</summary>
public sealed class Pair
{
    public Leaf First { get; set; } = new();
    public Leaf Second { get; set; } = new();
}

// Distinct-type chain for depth (not cycle) termination.
public sealed class Inner
{
    public int Value { get; set; }
}

public sealed class Middle
{
    public Inner? Inner { get; set; }
}

public sealed class Outer
{
    public Middle? Middle { get; set; }
}

/// <summary>Self-referential collection — terminates as empty.</summary>
public sealed class TreeNode
{
    public Guid Id { get; set; }
    public List<TreeNode> Children { get; set; } = new();
}

public sealed class IntCollections
{
    public List<int> Numbers { get; set; } = new();
    public int[] ArrayValues { get; set; } = System.Array.Empty<int>();
    public IReadOnlyList<int> ReadOnlyValues { get; set; } = new List<int>();
}

public sealed class CancelNested
{
    public int Trigger { get; set; }
    public Leaf Child { get; set; } = new();
}

public sealed class CancelCollection
{
    public int Trigger { get; set; }
    public List<int> Items { get; set; } = new();
}

public sealed class CancelDerive
{
    public int Trigger { get; set; }
    public int Derived { get; set; }
}

/// <summary>A phone member resolves to the internal 555-01xx generator.</summary>
public sealed class PhoneHolder
{
    public string Phone { get; set; } = string.Empty;
}
