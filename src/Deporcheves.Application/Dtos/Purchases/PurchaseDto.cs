namespace Deporcheves.Application.Dtos.Purchases;

public class PurchaseDto
{
    public int Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}