using System.Collections.Frozen;

namespace Munchausen.Inference;

/// <summary>Coarse structural category of a member's declared type.</summary>
internal enum StructuralKind
{
    Scalar,
    Nested,
    Collection,
    Unsupported,
}

/// <summary>The recognized collection shapes Munchausen materializes.</summary>
internal enum CollectionShape
{
    None,
    Array,
    List,
    IList,
    ICollection,
    IEnumerable,
    IReadOnlyList,
    IReadOnlyCollection,
    Dictionary,
    IDictionary,
    IReadOnlyDictionary,
}

/// <summary>The result of classifying a member's declared type.</summary>
internal sealed record StructuralClassification(
    StructuralKind Kind,
    Type Type,
    CollectionShape Shape = CollectionShape.None,
    Type? ElementType = null,
    Type? KeyType = null,
    Type? ValueType = null);

/// <summary>
/// Classifies a member type as scalar, nested object, collection, or unsupported.
/// <c>byte[]</c> is special-cased as a scalar (16 random bytes), not a collection.
/// </summary>
internal static class StructuralClassifier
{
    private static readonly FrozenSet<Type> ScalarTypes = new[]
    {
        typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(decimal), typeof(char), typeof(string),
        typeof(Guid), typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly),
        typeof(TimeOnly), typeof(TimeSpan), typeof(Uri),
    }.ToFrozenSet();

    private static readonly FrozenSet<Type> UnsupportedValueTypes = new[]
    {
        typeof(nint), typeof(nuint), typeof(Half), typeof(Int128), typeof(UInt128),
    }.ToFrozenSet();

    public static StructuralClassification Classify(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        // byte[] is a scalar, not a collection.
        if (type == typeof(byte[]))
        {
            return new StructuralClassification(StructuralKind.Scalar, type);
        }

        if (UnsupportedValueTypes.Contains(type) || type.IsPointer || type.IsByRef)
        {
            return new StructuralClassification(StructuralKind.Unsupported, type);
        }

        Type? underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
        {
            // Nullable<T> classifies as its underlying type does (null only via v1.1).
            StructuralClassification inner = Classify(underlying);
            return inner with { Type = type };
        }

        if (type.IsEnum || ScalarTypes.Contains(type))
        {
            return new StructuralClassification(StructuralKind.Scalar, type);
        }

        if (type.IsArray)
        {
            return type.GetArrayRank() == 1
                ? new StructuralClassification(
                    StructuralKind.Collection, type, CollectionShape.Array, type.GetElementType())
                : new StructuralClassification(StructuralKind.Unsupported, type);
        }

        if (type.IsGenericType && TryClassifyGeneric(type, out StructuralClassification? collection))
        {
            return collection;
        }

        if (type.IsInterface || type.IsAbstract)
        {
            return new StructuralClassification(StructuralKind.Unsupported, type);
        }

        return new StructuralClassification(StructuralKind.Nested, type);
    }

    private static bool TryClassifyGeneric(Type type, out StructuralClassification classification)
    {
        Type definition = type.GetGenericTypeDefinition();
        Type[] arguments = type.GetGenericArguments();

        CollectionShape dictionaryShape =
            definition == typeof(Dictionary<,>) ? CollectionShape.Dictionary
            : definition == typeof(IDictionary<,>) ? CollectionShape.IDictionary
            : definition == typeof(IReadOnlyDictionary<,>) ? CollectionShape.IReadOnlyDictionary
            : CollectionShape.None;

        if (dictionaryShape != CollectionShape.None)
        {
            classification = new StructuralClassification(
                StructuralKind.Collection,
                type,
                dictionaryShape,
                ElementType: typeof(KeyValuePair<,>).MakeGenericType(arguments),
                KeyType: arguments[0],
                ValueType: arguments[1]);
            return true;
        }

        CollectionShape sequenceShape =
            definition == typeof(List<>) ? CollectionShape.List
            : definition == typeof(IList<>) ? CollectionShape.IList
            : definition == typeof(ICollection<>) ? CollectionShape.ICollection
            : definition == typeof(IEnumerable<>) ? CollectionShape.IEnumerable
            : definition == typeof(IReadOnlyList<>) ? CollectionShape.IReadOnlyList
            : definition == typeof(IReadOnlyCollection<>) ? CollectionShape.IReadOnlyCollection
            : CollectionShape.None;

        if (sequenceShape != CollectionShape.None)
        {
            classification = new StructuralClassification(
                StructuralKind.Collection, type, sequenceShape, ElementType: arguments[0]);
            return true;
        }

        classification = null!;
        return false;
    }
}
