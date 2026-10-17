using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Dtos.Cash;

public class CashMovementRequest
{
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}