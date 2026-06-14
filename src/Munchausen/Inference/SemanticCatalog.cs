namespace Munchausen.Inference;

/// <summary>
/// The v1.0 semantic candidate table, mirroring INFERENCE_CATALOG.md in document
/// order. This file and the catalog document change together; a catalog change is
/// a seeded-output (minor-version) event. <c>SemanticCatalogConformanceTests</c>
/// asserts this table equals the document.
/// </summary>
internal static class SemanticCatalog
{
    private static readonly Type[] Str = { typeof(string) };
    private static readonly Type[] Int = { typeof(int) };
    private static readonly Type[] Dec = { typeof(decimal) };
    private static readonly Type[] Dbl = { typeof(double) };
    private static readonly Type[] Dates = { typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly) };

    private static readonly string[] PersonHints =
        { "person", "user", "customer", "employee", "contact", "owner", "author", "member", "account" };
    private static readonly string[] ProductHints = { "product", "item", "sku", "article" };
    private static readonly string[] CategoryHints = { "product", "item", "commerce", "order" };
    private static readonly string[] VehicleHints = { "car", "vehicle", "auto", "truck", "fleet" };

    private const InferenceConfidence High = InferenceConfidence.High;
    private const InferenceConfidence Medium = InferenceConfidence.Medium;
    private const InferenceConfidence Low = InferenceConfidence.Low;

    public static IReadOnlyList<SemanticCandidate> Entries { get; } = new SemanticCandidate[]
    {
        // Person
        new(new[] { "firstname", "givenname", "forename" }, null, Str, High, "Name.First"),
        new(new[] { "lastname", "surname", "familyname" }, null, Str, High, "Name.Last"),
        new(new[] { "fullname" }, null, Str, High, "Name.FullName"),
        new(new[] { "name" }, PersonHints, Str, High, "Name.FullName"),
        new(new[] { "name" }, ProductHints, Str, High, "Commerce.ProductName"),
        new(new[] { "age" }, null, Int, High, "int 18-80"),

        // Contact and Internet
        new(new[] { "email", "emailaddress", "mail" }, null, Str, High, "Internet.Email"),
        new(new[] { "phone", "phonenumber", "telephone", "mobile", "cellphone" }, null, Str, High, "internal phone"),
        new(new[] { "username", "login", "handle", "nickname" }, null, Str, High, "Internet.UserName"),
        new(new[] { "url", "website", "homepage", "link" }, null, Str, High, "Internet.Url"),
        new(new[] { "domain", "domainname", "hostname" }, null, Str, High, "Internet.DomainName"),
        new(new[] { "ipaddress", "ip" }, null, Str, Medium, "Internet.IpAddress"),

        // Address
        new(new[] { "streetaddress", "street", "addressline1", "address" }, null, Str, High,
            "Address.StreetAddress", ReducedConfidenceNames: new[] { "address" }),
        new(new[] { "city", "town" }, null, Str, High, "Address.City"),
        new(new[] { "state", "province", "region" }, null, Str, Medium, "Address.State"),
        new(new[] { "postalcode", "zipcode", "zip", "postcode" }, null, Str, High, "Address.PostalCode"),
        new(new[] { "country", "countryname" }, null, Str, High, "Address.Country"),
        new(new[] { "countrycode", "isocountrycode" }, null, Str, High, "Address.CountryCode"),
        new(new[] { "latitude", "lat" }, null, Dbl, High, "Address.Latitude"),
        new(new[] { "longitude", "lng", "lon" }, null, Dbl, High, "Address.Longitude"),

        // Dates
        new(new[] { "createdat", "createdon", "created", "creationdate" }, null, Dates, High, "Date.Past(2)"),
        new(new[] { "updatedat", "modifiedat", "updatedon", "lastmodified" }, null, Dates, High, "Date.Recent(30)"),
        new(new[] { "deletedat", "removedat" }, null, Dates, High, "Date.Recent(30)"),
        new(new[] { "birthdate", "dateofbirth", "dob", "birthday" }, null, Dates, High, "Date.BirthDate()"),
        new(new[] { "expiresat", "expirydate", "expirationdate", "validuntil" }, null, Dates, High, "Date.Future(2)"),
        new(new[] { "duedate", "dueat", "deadline" }, null, Dates, High, "Date.Soon(60)"),
        new(new[] { "startdate", "startedat", "begindate" }, null, Dates, Medium, "Date.Past(1)"),
        new(new[] { "enddate", "endedat", "finishdate" }, null, Dates, Medium, "Date.Future(1)"),

        // Commerce and Identifiers
        new(new[] { "price", "unitprice", "listprice" }, null, Dec, High, "Commerce.Price()"),
        new(new[] { "cost", "amount", "total", "subtotal" }, null, Dec, Medium, "Commerce.Price()"),
        new(new[] { "sku", "productcode" }, null, Str, High, "Commerce.Sku"),
        new(new[] { "category", "department" }, CategoryHints, Str, Medium, "Commerce.Category"),
        new(new[] { "currency", "currencycode" }, null, Str, High, "Commerce.CurrencyCode"),
        new(new[] { "id", "identifier" }, null, Str, Medium, "internal guid-string"),
        new(new[] { "code", "referencecode", "reference" }, null, Str, Low, "internal short-code"),
        new(new[] { "quantity", "count", "qty" }, null, Int, Medium, "int 1-100"),

        // Vehicle (hint-gated; per-row no-hint overrides win over rule #4)
        new(new[] { "make", "manufacturer" }, VehicleHints, Str, High, "Vehicle.Make", NoHintConfidence: Low),
        new(new[] { "model" }, VehicleHints, Str, High, "Vehicle.Model", NoHintConfidence: Low),
        new(new[] { "year" }, VehicleHints, Int, High, "Vehicle.Year",
            NoHintConfidence: Medium, NoHintGenerator: "internal recent-year"),
        new(new[] { "vin" }, null, Str, High, "Vehicle.Vin"),

        // Text
        new(new[] { "description", "summary", "details" }, null, Str, Medium, "Lorem.Paragraph"),
        new(new[] { "notes", "comment", "comments", "remarks" }, null, Str, Medium, "Lorem.Sentence"),
        new(new[] { "title", "subject", "heading" }, null, Str, Medium, "Lorem.Words(3)"),
        new(new[] { "bio", "about", "biography" }, null, Str, Medium, "Lorem.Paragraph"),
    };
}
