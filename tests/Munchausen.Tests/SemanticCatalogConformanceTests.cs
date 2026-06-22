using Munchausen.Inference;
using Xunit;

namespace Munchausen.Tests;

/// <summary>
/// Independent transcription of INFERENCE_CATALOG.md's semantic table, compared
/// one-to-one against the implemented catalog (names, hints, value types, base
/// confidence). Makes the catalog executable: drift in either direction fails.
/// </summary>
public sealed class SemanticCatalogConformanceTests
{
    // (names, hints, type-tag, base-confidence) per INFERENCE_CATALOG.md, in order.
    private static readonly (string Names, string? Hints, string Tag, string Base)[] Expected =
    {
        // Person
        ("firstname,givenname,forename", null, "str", "High"),
        ("lastname,surname,familyname", null, "str", "High"),
        ("fullname", null, "str", "High"),
        ("name", "person,user,customer,employee,contact,owner,author,member,account", "str", "High"),
        ("name", "product,item,sku,article", "str", "High"),
        ("age", null, "int", "High"),

        // Contact and Internet
        ("email,emailaddress,mail", null, "str", "High"),
        ("phone,phonenumber,telephone,mobile,cellphone", null, "str", "High"),
        ("username,login,handle,nickname", null, "str", "High"),
        ("url,website,homepage,link", null, "str", "High"),
        ("domain,domainname,hostname", null, "str", "High"),
        ("ipaddress,ip", null, "str", "Medium"),

        // Address
        ("streetaddress,street,addressline1,address", null, "str", "High"),
        ("city,town", null, "str", "High"),
        ("state,province,region", null, "str", "Medium"),
        ("postalcode,zipcode,zip,postcode", null, "str", "High"),
        ("country,countryname", null, "str", "High"),
        ("countrycode,isocountrycode", null, "str", "High"),
        ("latitude,lat", null, "dbl", "High"),
        ("longitude,lng,lon", null, "dbl", "High"),

        // Dates
        ("createdat,createdon,created,creationdate", null, "dates", "High"),
        ("updatedat,modifiedat,updatedon,lastmodified", null, "dates", "High"),
        ("deletedat,removedat", null, "dates", "High"),
        ("birthdate,dateofbirth,dob,birthday", null, "dates", "High"),
        ("expiresat,expirydate,expirationdate,validuntil", null, "dates", "High"),
        ("duedate,dueat,deadline", null, "dates", "High"),
        ("startdate,startedat,begindate", null, "dates", "Medium"),
        ("enddate,endedat,finishdate", null, "dates", "Medium"),

        // Commerce and Identifiers
        ("price,unitprice,listprice", null, "dec", "High"),
        ("cost,amount,total,subtotal", null, "dec", "Medium"),
        ("sku,productcode", null, "str", "High"),
        ("category,department", "product,item,commerce,order", "str", "Medium"),
        ("currency,currencycode", null, "str", "High"),
        ("id,identifier", null, "str", "Medium"),
        ("code,referencecode,reference", null, "str", "Low"),
        ("quantity,count,qty", null, "int", "Medium"),

        // Vehicle
        ("make,manufacturer", "car,vehicle,auto,truck,fleet", "str", "High"),
        ("model", "car,vehicle,auto,truck,fleet", "str", "High"),
        ("year", "car,vehicle,auto,truck,fleet", "int", "High"),
        ("vin", null, "str", "High"),

        // Text
        ("description,summary,details", null, "str", "Medium"),
        ("notes,comment,comments,remarks", null, "str", "Medium"),
        ("title,subject,heading", null, "str", "Medium"),
        ("bio,about,biography", null, "str", "Medium"),
    };

    [Fact]
    public void SemanticCatalog_MatchesDocumentRowsInOrder()
    {
        Assert.Equal(Expected.Length, SemanticCatalog.Entries.Count);

        for (int i = 0; i < Expected.Length; i++)
        {
            (string names, string? hints, string tag, string baseConfidence) = Expected[i];
            SemanticCandidate candidate = SemanticCatalog.Entries[i];

            Assert.Equal(names, string.Join(",", candidate.Names));
            Assert.Equal(hints, candidate.Hints is null ? null : string.Join(",", candidate.Hints));
            Assert.Equal(tag, TypeTag(candidate.ValueTypes));
            Assert.Equal(baseConfidence, candidate.BaseConfidence.ToString());
        }
    }

    [Fact]
    public void EveryCatalogNameIsAlreadyNormalized()
    {
        foreach (SemanticCandidate candidate in SemanticCatalog.Entries)
        {
            foreach (string name in candidate.Names)
            {
                Assert.Equal(MemberNameNormalizer.Normalize(name), name);
            }
        }
    }

    [Fact]
    public void TypeDefaults_CoverScalarsAndExcludeUnsupportedTypes()
    {
        Assert.Equal("0-10000", TypeDefaults.Table[typeof(int)]);
        Assert.Equal("Lorem.Words(2)", TypeDefaults.Table[typeof(string)]);
        Assert.Equal("16 random bytes", TypeDefaults.Table[typeof(byte[])]);

        Assert.DoesNotContain(typeof(nint), TypeDefaults.Table.Keys);
        Assert.DoesNotContain(typeof(Int128), TypeDefaults.Table.Keys);

        Assert.Equal("0-10000", TypeDefaults.DescribeFor(StructuralClassifier.Classify(typeof(int?))));
        Assert.Equal(
            "uniform over defined values",
            TypeDefaults.DescribeFor(StructuralClassifier.Classify(typeof(DayOfWeek))));
        Assert.Null(TypeDefaults.DescribeFor(StructuralClassifier.Classify(typeof(nint))));
    }

    private static string TypeTag(Type[] types)
    {
        if (types.SequenceEqual(new[] { typeof(string) }))
        {
            return "str";
        }

        if (types.SequenceEqual(new[] { typeof(int) }))
        {
            return "int";
        }

        if (types.SequenceEqual(new[] { typeof(decimal) }))
        {
            return "dec";
        }

        if (types.SequenceEqual(new[] { typeof(double) }))
        {
            return "dbl";
        }

        if (types.SequenceEqual(new[] { typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly) }))
        {
            return "dates";
        }

        return "?";
    }
}
