using System.Collections.Concurrent;
using Munchausen.Runtime;

namespace Munchausen.Runtime;

/// <summary>
/// Build-time bound generators for the pure-PRNG type defaults (no datasets, no
/// reference time). Dataset-backed defaults (string, Uri, the date/time family)
/// and all semantic generators bind in M7; here they resolve to null so the
/// compiler keeps an unbound placeholder.
/// </summary>
internal static class TypeDefaultGenerators
{
    private static readonly ConcurrentDictionary<Type, Func<GenerationContext, object?>> EnumGenerators = new();

    private static readonly Dictionary<Type, Func<GenerationContext, object?>> Generators = new()
    {
        [typeof(bool)] = context => context.Random.Bool(),
        [typeof(byte)] = context => (byte)context.Random.Int(0, 255),
        [typeof(sbyte)] = context => (sbyte)context.Random.Int(0, 100),
        [typeof(short)] = context => (short)context.Random.Int(0, 1000),
        [typeof(ushort)] = context => (ushort)context.Random.Int(0, 1000),
        [typeof(int)] = context => context.Random.Int(0, 10000),
        [typeof(uint)] = context => (uint)context.Random.Long(0, 10000),
        [typeof(long)] = context => context.Random.Long(0, 1000000),
        [typeof(ulong)] = context => (ulong)context.Random.Long(0, 1000000),
        [typeof(float)] = context => (float)Math.Round(context.Random.Double(0, 10000), 4),
        [typeof(double)] = context => Math.Round(context.Random.Double(0, 10000), 4),
        [typeof(decimal)] = context => context.Random.Decimal(0m, 10000m, 2),
        [typeof(char)] = context => (char)context.Random.Int('a', 'z'),
        [typeof(Guid)] = context => context.Random.Guid(),
        [typeof(byte[])] = context => context.Random.Bytes(16),
    };

    /// <summary>The pure generator for <paramref name="type"/>, or null if it binds in M7.</summary>
    public static Func<GenerationContext, object?>? For(Type type)
    {
        if (type.IsEnum)
        {
            return EnumGenerators.GetOrAdd(type, static enumType =>
            {
                Array values = Enum.GetValues(enumType);
                return context => values.GetValue(context.Random.Int(0, values.Length - 1));
            });
        }

        return Generators.GetValueOrDefault(type);
    }
}
