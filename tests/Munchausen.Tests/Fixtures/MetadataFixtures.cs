using System.ComponentModel.DataAnnotations;

namespace Munchausen.Tests.Fixtures;

/// <summary>Covers the nullable-reference-type detection matrix.</summary>
public sealed class NullabilityFixture
{
    public string NonNullableRef { get; set; } = string.Empty;

    public string? NullableRef { get; set; }

    public int NonNullableValue { get; set; }

    public int? NullableValue { get; set; }

    public required string RequiredModifier { get; set; }

    // Nullable ref whose required-ness comes purely from the attribute.
    [Required]
    public string? RequiredAttribute { get; set; }
}

#nullable disable
/// <summary>A reference member compiled without nullable annotations (oblivious).</summary>
public sealed class ObliviousFixture
{
    public string ObliviousRef { get; set; }
}
#nullable restore

/// <summary>Covers the writability matrix.</summary>
public sealed class WritabilityFixture
{
    public int Writable { get; set; }

    public string InitOnly { get; init; } = string.Empty;

    public int Computed => 42;
}
