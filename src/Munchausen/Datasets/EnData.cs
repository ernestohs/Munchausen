namespace Munchausen.Datasets;

/// <summary>
/// English ("en") data tables: internal static arrays, no resource files. Contents
/// may grow in minor versions (a seeded-output event); the shape never changes.
/// </summary>
internal static class EnData
{
    public static readonly string[] FirstNames =
    {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
        "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa",
        "Matthew", "Margaret", "Anthony", "Betty", "Mark", "Sandra", "Ada", "Grace",
    };

    public static readonly string[] LastNames =
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson",
        "White", "Harris", "Clark", "Lewis", "Walker", "Hall", "Young", "King", "Wright",
    };

    public static readonly string[] StreetNames =
    {
        "Maple", "Oak", "Pine", "Cedar", "Elm", "Washington", "Lake", "Hill", "Park",
        "Sunset", "Lincoln", "Adams", "Jefferson", "Highland", "River", "Spring", "Church",
        "Meadow", "Forest", "Willow", "Birch", "Chestnut", "Walnut", "Aspen", "Juniper",
    };

    public static readonly string[] StreetSuffixes =
    {
        "Street", "Avenue", "Boulevard", "Drive", "Lane", "Road", "Court", "Place", "Way", "Terrace",
    };

    public static readonly string[] Cities =
    {
        "Springfield", "Riverside", "Franklin", "Greenville", "Bristol", "Clinton", "Fairview",
        "Salem", "Madison", "Georgetown", "Arlington", "Ashland", "Burlington", "Manchester",
        "Oxford", "Dayton", "Auburn", "Milton", "Newport", "Kingston", "Cleveland", "Marion",
    };

    public static readonly string[] States =
    {
        "Alabama", "Alaska", "Arizona", "California", "Colorado", "Florida", "Georgia",
        "Illinois", "Indiana", "Kansas", "Kentucky", "Maine", "Maryland", "Michigan",
        "Minnesota", "Nevada", "Ohio", "Oregon", "Tennessee", "Texas", "Utah", "Virginia",
        "Washington", "Wisconsin", "Wyoming",
    };

    // (English short name, ISO 3166-1 alpha-2)
    public static readonly (string Name, string Code)[] Countries =
    {
        ("United States", "US"), ("Canada", "CA"), ("Mexico", "MX"), ("Brazil", "BR"),
        ("United Kingdom", "GB"), ("France", "FR"), ("Germany", "DE"), ("Spain", "ES"),
        ("Italy", "IT"), ("Portugal", "PT"), ("Netherlands", "NL"), ("Sweden", "SE"),
        ("Norway", "NO"), ("Japan", "JP"), ("Australia", "AU"), ("India", "IN"),
        ("Argentina", "AR"), ("Chile", "CL"), ("Ireland", "IE"), ("Poland", "PL"),
    };

    public static readonly string[] EmailDomains = { "example.com", "example.org", "example.net" };

    public static readonly string[] LoremWords =
    {
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
        "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
        "magna", "aliqua", "enim", "ad", "minim", "veniam", "quis", "nostrud",
        "exercitation", "ullamco", "laboris", "nisi", "aliquip", "ex", "ea", "commodo",
        "consequat", "duis", "aute", "irure", "in", "reprehenderit", "voluptate", "velit",
        "esse", "cillum", "fugiat", "nulla", "pariatur", "excepteur", "sint", "occaecat",
    };

    public static readonly string[] ProductAdjectives =
    {
        "Ergonomic", "Rustic", "Sleek", "Refined", "Handcrafted", "Intelligent", "Modern",
        "Practical", "Compact", "Durable", "Elegant", "Lightweight", "Premium", "Versatile",
    };

    public static readonly string[] ProductMaterials =
    {
        "Steel", "Wooden", "Concrete", "Plastic", "Cotton", "Granite", "Rubber", "Leather",
        "Bronze", "Copper", "Aluminum", "Bamboo", "Ceramic", "Glass",
    };

    public static readonly string[] ProductNouns =
    {
        "Chair", "Table", "Lamp", "Shoes", "Hat", "Gloves", "Keyboard", "Mouse", "Bottle",
        "Backpack", "Watch", "Wallet", "Notebook", "Bench", "Shelf", "Mug", "Pillow",
    };

    public static readonly string[] Categories =
    {
        "Electronics", "Books", "Home", "Garden", "Toys", "Grocery", "Clothing", "Shoes",
        "Jewelry", "Sports", "Outdoors", "Automotive", "Industrial", "Health", "Beauty",
    };

    public static readonly string[] CurrencyCodes =
    {
        "USD", "EUR", "GBP", "JPY", "CAD", "AUD", "CHF", "CNY", "MXN", "BRL",
        "SEK", "NOK", "INR", "ZAR", "NZD", "SGD",
    };

    public static readonly string[] VehicleMakes =
    {
        "Toyota", "Honda", "Ford", "Chevrolet", "Nissan", "Volkswagen", "BMW", "Mercedes",
        "Audi", "Hyundai", "Kia", "Mazda", "Subaru", "Volvo",
    };

    public static readonly string[] VehicleModels =
    {
        "Corolla", "Camry", "Civic", "Accord", "Focus", "Mustang", "Malibu", "Altima",
        "Golf", "Passat", "Sentra", "Outback", "Forester", "Elantra", "Sorento", "Mazda3",
    };
}
