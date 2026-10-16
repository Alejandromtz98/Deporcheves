using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Dtos.Sales;

public class SaleDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}