using System.ComponentModel.DataAnnotations;
using Munchausen.Metadata;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class ModelMetadataTests
{
    private static ModelMetadata Metadata<T>() =>
        new ReflectionModelMetadataProvider().GetMetadata(typeof(T));

    private static MemberMetadata Member<T>(string name) =>
        Metadata<T>().Members.Single(member => member.Name == name);

    [Fact]
    public void Members_ComeBackInDeclarationOrder()
    {
        string[] owner = Metadata<Owner>().Members.Select(member => member.Name).ToArray();
        Assert.Equal(new[] { "Id", "FirstName", "LastName", "Email" }, owner);

        string[] customer = Metadata<Customer>().Members.Select(member => member.Name).ToArray();
        Assert.Equal(new[] { "Id", "FirstName", "LastName", "Email", "Orders" }, customer);

        // DeclarationOrder mirrors the sequence.
        IReadOnlyList<MemberMetadata> members = Metadata<Owner>().Members;
        for (int i = 0; i < members.Count; i++)
        {
            Assert.Equal(i, members[i].DeclarationOrder);
        }
    }

    [Theory]
    [InlineData("NonNullableRef", "NonNullable")]
    [InlineData("NullableRef", "Nullable")]
    [InlineData("NonNullableValue", "NonNullable")]
    [InlineData("NullableValue", "Nullable")]
    [InlineData("RequiredModifier", "NonNullable")]
    [InlineData("RequiredAttribute", "Nullable")]
    public void Nullability_IsDetectedPerMember(string memberName, string expected)
    {
        Assert.Equal(expected, Member<NullabilityFixture>(memberName).Nullability.ToString());
    }

    [Fact]
    public void Nullability_ObliviousReferenceIsDetected()
    {
        Assert.Equal(NullabilityKind.Oblivious, Member<ObliviousFixture>("ObliviousRef").Nullability);
    }

    [Theory]
    [InlineData("RequiredModifier", true)]   // C# required modifier
    [InlineData("RequiredAttribute", true)]  // [Required] DataAnnotation
    [InlineData("NonNullableRef", false)]
    [InlineData("NullableValue", false)]
    public void IsRequired_DetectsModifierAndAttribute(string memberName, bool expected)
    {
        Assert.Equal(expected, Member<NullabilityFixture>(memberName).IsRequired);
    }

    [Theory]
    [InlineData("Writable", "Writable")]
    [InlineData("InitOnly", "InitOnly")]
    [InlineData("Computed", "ReadOnly")]
    public void Writability_IsClassified(string memberName, string expected)
    {
        Assert.Equal(expected, Member<WritabilityFixture>(memberName).Writability.ToString());
    }

    [Fact]
    public void Attributes_AreCaptured()
    {
        IReadOnlyList<Attribute> emailAttributes = Member<User>("Email").Attributes;
        Assert.Contains(emailAttributes, attribute => attribute is RequiredAttribute);
        Assert.Contains(emailAttributes, attribute => attribute is EmailAddressAttribute);

        Assert.Contains(Member<User>("Age").Attributes, attribute => attribute is RangeAttribute);
    }

    [Fact]
    public void Constructors_AreCapturedWithParameters()
    {
        ConstructorMetadata constructor = Assert.Single(
            Metadata<CarRecord>().Constructors,
            candidate => candidate.Parameters.Count == 4);

        Assert.Equal(
            new[] { "Id", "Make", "Model", "Year" },
            constructor.Parameters.Select(parameter => parameter.Name));
    }

    [Fact]
    public void NullableAnnotationContext_IsEnabledForAnnotatedModel()
    {
        Assert.True(Metadata<Owner>().IsNullableAnnotationContextEnabled);
    }

    [Fact]
    public void Metadata_IsCachedPerType()
    {
        var provider = new ReflectionModelMetadataProvider();
        Assert.Same(provider.GetMetadata(typeof(Owner)), provider.GetMetadata(typeof(Owner)));
    }
}
