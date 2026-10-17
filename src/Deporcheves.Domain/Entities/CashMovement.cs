using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class CashMovement
{
    public int Id { get; set; }
    public int CashSessionId { get; set; }
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CashSession CashSession { get; set; } = null!;
}