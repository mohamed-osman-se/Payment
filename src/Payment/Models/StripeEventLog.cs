using System.ComponentModel.DataAnnotations;

namespace Payment.Models;

public class StripeEventLog
{
    public int Id { get; set; }

    [Required]
    public string EventId { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;

    public string? PaymentIntentId { get; set; }

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public string? RawJson { get; set; }
}



