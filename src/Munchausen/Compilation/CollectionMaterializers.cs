using System.Reflection;

namespace Munchausen.Compilation;

/// <summary>
/// Builds a reflection-free materializer delegate for a collection shape at compile
/// time (one reflection call here; none at generation time). Sequences become a
/// typed array or <see cref="List{T}"/>; dictionaries materialize empty in v1.0
/// (key generation, usually string-based, binds with the datasets in M7).
/// </summary>
internal static class CollectionMaterializers
{
    public static Func<IReadOnlyList<object?>, object> ForArray(Type elementType) =>
        Bind(nameof(ToArray), elementType);

    public static Func<IReadOnlyList<object?>, object> ForList(Type elementType) =>
        Bind(nameof(ToList), elementType);

    public static Func<IReadOnlyList<object?>, object> ForEmptyDictionary(Type keyType, Type valueType)
    {
        MethodInfo method = typeof(CollectionMaterializers)
            .GetMethod(nameof(EmptyDictionary), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(keyType, valueType);
        return method.CreateDelegate<Func<IReadOnlyList<object?>, object>>();
    }

    public static object ToArray<T>(IReadOnlyList<object?> elements)
    {
        var array = new T[elements.Count];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (T)elements[i]!;
        }

        return array;
    }

    public static object ToList<T>(IReadOnlyList<object?> elements)
    {
        var list = new List<T>(elements.Count);
        foreach (object? element in elements)
        {
            list.Add((T)element!);
        }

        return list;
    }

    public static object EmptyDictionary<TKey, TValue>(IReadOnlyList<object?> elements)
        where TKey : notnull => new Dictionary<TKey, TValue>();

    private static Func<IReadOnlyList<object?>, object> Bind(string methodName, Type elementType)
    {
        MethodInfo method = typeof(CollectionMaterializers)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(elementType);
        return method.CreateDelegate<Func<IReadOnlyList<object?>, object>>();
    }
}
