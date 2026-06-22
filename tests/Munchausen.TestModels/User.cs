using System.ComponentModel.DataAnnotations;

namespace Munchausen.TestModels;

public sealed class User
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Range(18, 100)]
    public int Age { get; set; }
}
