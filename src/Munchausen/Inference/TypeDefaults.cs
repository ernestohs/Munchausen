using System.Collections.Frozen;

namespace Munchausen.Inference;

/// <summary>
/// The v1.0 TypeStage defaults, mirroring INFERENCE_CATALOG.md's TypeDefaults
/// table. Descriptions are the Explain-facing generator names; the real
/// generators bind in M7. Values favor "useful, human-plausible" over full range.
/// </summary>
internal static class TypeDefaults
{
    private static readonly FrozenDictionary<Type, string> Descriptions = new Dictionary<Type, string>
    {
        [typeof(bool)] = "50/50",
        [typeof(byte)] = "0-255",
        [typeof(sbyte)] = "0-100",
        [typeof(short)] = "0-1000",
        [typeof(ushort)] = "0-1000",
        [typeof(int)] = "0-10000",
        [typeof(uint)] = "0-10000",
        [typeof(long)] = "0-1000000",
        [typeof(ulong)] = "0-1000000",
        [typeof(float)] = "0-10000, 4 decimals",
        [typeof(double)] = "0-10000, 4 decimals",
        [typeof(decimal)] = "0-10000, 2 decimals",
        [typeof(char)] = "'a'-'z'",
        [typeof(string)] = "Lorem.Words(2)",
        [typeof(Guid)] = "Random.Guid",
        [typeof(DateTime)] = "Date.Recent(365), UTC",
        [typeof(DateTimeOffset)] = "Date.Recent(365), UTC",
        [typeof(DateOnly)] = "date part of Date.Recent(365)",
        [typeof(TimeOnly)] = "uniform over the day",
        [typeof(TimeSpan)] = "uniform 0-24h, second precision",
        [typeof(Uri)] = "absolute https URI from Internet.Url",
        [typeof(byte[])] = "16 random bytes",
    }.ToFrozenDictionary();

    /// <summary>The default generator description for a scalar classification, or null if unsupported.</summary>
    public static string? DescribeFor(StructuralClassification classification)
    {
        Type type = Nullable.GetUnderlyingType(classification.Type) ?? classification.Type;

        if (type.IsEnum)
        {
            return "uniform over defined values";
        }

        return Descriptions.GetValueOrDefault(type);
    }

    /// <summary>Exposed for the conformance test.</summary>
    public static IReadOnlyDictionary<Type, string> Table => Descriptions;
}
