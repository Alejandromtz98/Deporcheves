namespace Deporcheves.Web.Models;

public class PurchaseCartItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Subtotal => UnitCost * Quantity;
}