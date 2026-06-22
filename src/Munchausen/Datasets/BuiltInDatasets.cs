using Munchausen.Runtime;

namespace Munchausen.Datasets;

/// <summary>Factory for the built-in datasets reachable through <c>Dataset&lt;T&gt;()</c>.</summary>
internal static class BuiltInDatasets
{
    public static object? Create(Type type, DeterministicRandom random, DateTimeOffset referenceTime) =>
        type == typeof(RandomData) ? new RandomData(random)
        : type == typeof(NameData) ? new NameData(random)
        : type == typeof(InternetData) ? new InternetData(random)
        : type == typeof(AddressData) ? new AddressData(random)
        : type == typeof(DateData) ? new DateData(random, referenceTime)
        : type == typeof(LoremData) ? new LoremData(random)
        : type == typeof(VehicleData) ? new VehicleData(random, referenceTime)
        : type == typeof(CommerceData) ? new CommerceData(random)
        : null;
}
