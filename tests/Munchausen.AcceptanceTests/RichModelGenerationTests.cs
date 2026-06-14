using Munchausen;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.AcceptanceTests;

/// <summary>
/// End-to-end generation of the canonical models through the public surface, now
/// that the datasets bind the semantic and dataset-backed type generators.
/// </summary>
public sealed class RichModelGenerationTests
{
    [Fact]
    public void Car_GeneratesAFullGraph()
    {
        Car car = Lie.Define<Car>().Build().Generate(new GenerationOptions { Seed = 1 });

        Assert.NotEmpty(car.Make);     // semantic: Vehicle.Make (hint-gated by "Car")
        Assert.NotEmpty(car.Model);    // semantic: Vehicle.Model
        Assert.InRange(car.Year, 1990, DateTimeOffset.UtcNow.Year + 1);
        Assert.True(car.Price >= 0);

        Assert.NotNull(car.Owner);
        Assert.NotEmpty(car.Owner.FirstName);
        Assert.NotEmpty(car.Owner.LastName);
        Assert.Contains("@", car.Owner.Email);
    }

    [Fact]
    public void Customer_WithOrderCollection_Generates()
    {
        Customer customer = Lie.Define<Customer>().Build().Generate(new GenerationOptions { Seed = 7 });

        Assert.NotEmpty(customer.FirstName);
        Assert.Contains("@", customer.Email);
        Assert.NotNull(customer.Orders);
        Assert.All(customer.Orders, order => Assert.NotNull(order.Items));
    }
}
