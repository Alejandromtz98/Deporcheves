namespace Deporcheves.Application.Dtos.Purchases;

public class CreatePurchaseItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}