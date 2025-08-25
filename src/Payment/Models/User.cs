using System.ComponentModel.DataAnnotations;

namespace Payment.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    public string? Name { get; set; }

    public ICollection<Order>? Orders { get; set; }
}


