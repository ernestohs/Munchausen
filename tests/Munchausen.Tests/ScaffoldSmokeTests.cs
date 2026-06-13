using Munchausen.TestModels;
using Xunit;

namespace Munchausen.Tests;

public sealed class ScaffoldSmokeTests
{
    [Fact]
    public void SharedTestModelsAreUsable()
    {
        var employee = new Employee { FirstName = "Ada", Manager = new Employee() };
        var car = new CarRecord(Guid.NewGuid(), "Saab", "900", 1989);
        var profile = new Profile { DisplayName = "ada" };

        Assert.NotNull(employee.Manager);
        Assert.Equal(1989, car.Year);
        Assert.Null(profile.Bio);
    }
}
