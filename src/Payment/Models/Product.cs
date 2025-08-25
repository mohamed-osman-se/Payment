using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Models;


public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? ImageUrl { get; set; }
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public ICollection<Order>? Orders { get; set; }
}


