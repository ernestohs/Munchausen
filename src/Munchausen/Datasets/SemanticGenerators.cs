namespace Munchausen.Datasets;

/// <summary>
/// Binds each semantic-catalog generator name (INFERENCE_CATALOG.md) to a real
/// implementation over the datasets and internal generators. Date generators
/// yield <see cref="DateTimeOffset"/>; the compiler adapts them to the member's
/// declared date type.
/// </summary>
internal static class SemanticGenerators
{
    private static readonly Dictionary<string, Func<GenerationContext, object?>> Map = new()
    {
        ["Name.First"] = c => c.Name.First(),
        ["Name.Last"] = c => c.Name.Last(),
        ["Name.FullName"] = c => c.Name.FullName(),
        ["Commerce.ProductName"] = c => c.Dataset<CommerceData>().ProductName(),
        ["int 18-80"] = c => c.Random.Int(18, 80),

        ["Internet.Email"] = c => c.Internet.Email(),
        ["internal phone"] = InternalGenerators.Phone,
        ["Internet.UserName"] = c => c.Internet.UserName(),
        ["Internet.Url"] = c => c.Internet.Url(),
        ["Internet.DomainName"] = c => c.Internet.DomainName(),
        ["Internet.IpAddress"] = c => c.Internet.IpAddress(),

        ["Address.StreetAddress"] = c => c.Address.StreetAddress(),
        ["Address.City"] = c => c.Address.City(),
        ["Address.State"] = c => c.Address.State(),
        ["Address.PostalCode"] = c => c.Address.PostalCode(),
        ["Address.Country"] = c => c.Address.Country(),
        ["Address.CountryCode"] = c => c.Address.CountryCode(),
        ["Address.Latitude"] = c => c.Address.Latitude(),
        ["Address.Longitude"] = c => c.Address.Longitude(),

        ["Date.Past(2)"] = c => c.Date.Past(2),
        ["Date.Recent(30)"] = c => c.Date.Recent(30),
        ["Date.BirthDate()"] = c => c.Date.BirthDate(),
        ["Date.Future(2)"] = c => c.Date.Future(2),
        ["Date.Soon(60)"] = c => c.Date.Soon(60),
        ["Date.Past(1)"] = c => c.Date.Past(1),
        ["Date.Future(1)"] = c => c.Date.Future(1),

        ["Commerce.Price()"] = c => c.Dataset<CommerceData>().Price(),
        ["Commerce.Sku"] = c => c.Dataset<CommerceData>().Sku(),
        ["Commerce.Category"] = c => c.Dataset<CommerceData>().Category(),
        ["Commerce.CurrencyCode"] = c => c.Dataset<CommerceData>().CurrencyCode(),
        ["internal guid-string"] = InternalGenerators.GuidString,
        ["internal short-code"] = InternalGenerators.ShortCode,
        ["int 1-100"] = c => c.Random.Int(1, 100),

        ["Vehicle.Make"] = c => c.Dataset<VehicleData>().Make(),
        ["Vehicle.Model"] = c => c.Dataset<VehicleData>().Model(),
        ["Vehicle.Year"] = c => c.Dataset<VehicleData>().Year(),
        ["Vehicle.Vin"] = c => c.Dataset<VehicleData>().Vin(),
        ["internal recent-year"] = c => InternalGenerators.RecentYear(c),

        ["Lorem.Paragraph"] = c => c.Lorem.Paragraph(),
        ["Lorem.Sentence"] = c => c.Lorem.Sentence(),
        ["Lorem.Words(3)"] = c => c.Lorem.Words(3),
    };

    public static Func<GenerationContext, object?>? Resolve(string generatorName) =>
        Map.GetValueOrDefault(generatorName);
}
