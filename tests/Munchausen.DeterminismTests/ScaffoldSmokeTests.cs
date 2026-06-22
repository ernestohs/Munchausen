using Munchausen.TestModels;
using Xunit;

namespace Munchausen.DeterminismTests;

public sealed class ScaffoldSmokeTests
{
    [Fact]
    public void SharedTestModelsAreUsable()
    {
        var order = new Order { Items = [new Item { Quantity = 3 }] };

        Assert.Equal(3, order.Items[0].Quantity);
    }
}
