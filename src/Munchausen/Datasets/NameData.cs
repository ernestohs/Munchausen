using Munchausen.Datasets;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>Personal names (English).</summary>
public sealed class NameData
{
    private readonly DeterministicRandom _random;

    internal NameData(DeterministicRandom random) => _random = random;

    /// <summary>A given name.</summary>
    public string First() => _random.Pick(EnData.FirstNames);

    /// <summary>A family name.</summary>
    public string Last() => _random.Pick(EnData.LastNames);

    /// <summary>A full name composed as one <see cref="First"/> then one <see cref="Last"/> draw.</summary>
    public string FullName() => $"{First()} {Last()}";
}
