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
    public decimal PaidAmount { get; set; }
    public decimal Balance { get; set; }
    public bool IsFullyPaid => Balance <= 0.01m;
    public List<PurchaseItemDto> Items { get; set; } = new();
}