using System.Linq.Expressions;
using Munchausen.Compilation;
using Munchausen.TestModels;
using Xunit;

namespace Munchausen.Tests;

public sealed class ExpressionMemberResolverTests
{
    private static readonly ExpressionMemberResolver Resolver = new();

    [Fact]
    public void Resolves_PlainReferenceMember()
    {
        Expression<Func<Car, string>> expression = car => car.Make;

        ExpressionResolution result = Resolver.Resolve(expression);

        Assert.True(result.IsResolved);
        Assert.Equal("Make", result.Member!.Name);
    }

    [Fact]
    public void Resolves_PlainValueMember()
    {
        Expression<Func<Car, int>> expression = car => car.Year;

        ExpressionResolution result = Resolver.Resolve(expression);

        Assert.True(result.IsResolved);
        Assert.Equal("Year", result.Member!.Name);
    }

    [Fact]
    public void Resolves_BoxedValueMemberThroughConvert()
    {
        // Expression<Func<Car, object>> forces the compiler to insert a Convert node.
        Expression<Func<Car, object>> expression = car => car.Year;
        Assert.Equal(ExpressionType.Convert, expression.Body.NodeType);

        ExpressionResolution result = Resolver.Resolve(expression);

        Assert.True(result.IsResolved);
        Assert.Equal("Year", result.Member!.Name);
    }

    [Fact]
    public void Rejects_NestedPath()
    {
        Expression<Func<Car, string>> expression = car => car.Owner.FirstName;
        AssertRejected(expression, "Owner");
    }

    [Fact]
    public void Rejects_MethodCall()
    {
        Expression<Func<Car, string>> expression = car => car.Make.ToUpper();
        AssertRejected(expression, "ToUpper");
    }

    [Fact]
    public void Rejects_Indexer()
    {
        Expression<Func<Customer, Order>> expression = customer => customer.Orders[0];
        AssertRejected(expression, "Orders");
    }

    [Fact]
    public void Rejects_NonParameterRoot()
    {
        var other = new Car();
        Expression<Func<Car, string>> expression = car => other.Make;
        AssertRejected(expression, "Make");
    }

    [Fact]
    public void Rejects_Constant()
    {
        Expression<Func<Car, int>> expression = car => 5;
        AssertRejected(expression, "5");
    }

    private static void AssertRejected(LambdaExpression expression, string expectedTextFragment)
    {
        ExpressionResolution result = Resolver.Resolve(expression);

        Assert.False(result.IsResolved);
        Assert.Equal("LIE001", result.DiagnosticCode);
        Assert.NotNull(result.Message);
        Assert.Contains(expectedTextFragment, result.Message);
    }
}
