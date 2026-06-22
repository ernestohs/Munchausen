namespace Munchausen.TestModels;

public sealed class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Employee? Manager { get; set; }
}
