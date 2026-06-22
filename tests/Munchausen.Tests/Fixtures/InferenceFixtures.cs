namespace Munchausen.Tests.Fixtures;

/// <summary>Neutral model name (no hint substrings) for mode-filter tests.</summary>
public sealed class Widget
{
    public string Code { get; set; } = string.Empty;   // internal short-code, Low
    public string Email { get; set; } = string.Empty;  // Internet.Email, High
    public string State { get; set; } = string.Empty;  // Address.State, Medium
}

/// <summary>The documented Product.Code rejection case.</summary>
public sealed class Product
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
