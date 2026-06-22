using Munchausen.TestModels;
using Xunit;

namespace Munchausen.AcceptanceTests;

public sealed class ScaffoldSmokeTests
{
    [Fact]
    public void SharedTestModelsAreUsable()
    {
        var car = new Car { Make = "Saab", Owner = new Owner { FirstName = "Ada" } };

        Assert.Equal("Saab", car.Make);
        Assert.Equal("Ada", car.Owner.FirstName);
    }
}
