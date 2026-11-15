namespace Deporcheves.Domain.Entities;

public class SaleItem
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    // NUEVO: Descuento por línea (ej. descuento por cartón)
    public decimal Discount { get; set; } 

    // El subtotal ahora resta el descuento
    public decimal Subtotal => (Quantity * UnitPrice) - Discount;

    public Sale Sale { get; set; } = null!;
}