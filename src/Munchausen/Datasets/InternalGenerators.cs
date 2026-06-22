using System.Globalization;

namespace Munchausen.Datasets;

/// <summary>
/// Generators referenced by the semantic catalog but exposed on no public dataset
/// in v1.0 (DATASETS.md "Internal-Only Generators").
/// </summary>
internal static class InternalGenerators
{
    /// <summary>A fictional phone number in the 555-01xx range.</summary>
    public static string Phone(GenerationContext context) =>
        $"+1-555-01{context.Random.Int(0, 99).ToString("D2", CultureInfo.InvariantCulture)}";

    /// <summary>A guid-string identifier for string-typed <c>Id</c> members.</summary>
    public static string GuidString(GenerationContext context) => context.Random.Guid().ToString();

    /// <summary>A short code for string-typed <c>Code</c> members.</summary>
    public static string ShortCode(GenerationContext context) => context.Random.AlphaNumeric(8).ToUpperInvariant();

    /// <summary>A recent year for non-vehicle <c>Year</c> members.</summary>
    public static int RecentYear(GenerationContext context) =>
        context.Date.ReferenceTime.Year - context.Random.Int(0, 5);
}
