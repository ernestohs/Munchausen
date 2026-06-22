using Munchausen;
using Munchausen.Runtime;
using Munchausen.TestModels;
using Munchausen.Tests.Fixtures;
using Xunit;

namespace Munchausen.Tests;

public sealed class DatasetDeterminismTests
{
    [Fact]
    public void PhoneMember_UsesFictional555_01xxGenerator()
    {
        LieDefinition<PhoneHolder> definition = Lie.Define<PhoneHolder>().Build();
        for (int seed = 0; seed < 20; seed++)
        {
            PhoneHolder result = definition.Generate(new GenerationOptions { Seed = seed });
            Assert.Contains("555-01", result.Phone);
        }
    }

    [Fact]
    public void SameSeed_DatasetsProduceIdenticalSequences()
    {
        var first = new NameData(new DeterministicRandom(5));
        var second = new NameData(new DeterministicRandom(5));

        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(first.FullName(), second.FullName());
        }
    }

    [Fact]
    public void SameSeed_DatasetBackedModelsAreIdentical()
    {
        LieDefinition<Owner> definition = Lie.Define<Owner>().Build();

        Owner a = definition.Generate(new GenerationOptions { Seed = 777 });
        Owner b = definition.Generate(new GenerationOptions { Seed = 777 });

        Assert.Equal((a.Id, a.FirstName, a.LastName, a.Email), (b.Id, b.FirstName, b.LastName, b.Email));
    }
}
