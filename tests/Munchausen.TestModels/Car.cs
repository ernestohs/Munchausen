namespace Munchausen.TestModels;

public sealed class Car
{
    public Guid Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public Owner Owner { get; set; }
}
