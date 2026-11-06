using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class CapitalMovement
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public CapitalMovementType Type { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? AuthorizedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}