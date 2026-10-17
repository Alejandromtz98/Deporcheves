namespace Deporcheves.Application.Dtos.Purchases;

public class CreatePurchaseRequest
{
    public int SupplierId { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseItemRequest> Items { get; set; } = new();
}