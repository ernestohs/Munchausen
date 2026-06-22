namespace Munchausen.TestModels;

public sealed class Order
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal Total { get; set; }
    public List<Item> Items { get; set; }
}
