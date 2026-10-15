namespace Deporcheves.Domain.Entities;

/// <summary>
/// Linea individual de una venta. Representa un producto vendido en una cantidad
/// especifica a un precio especifico.
/// 
/// Es un snapshot historico: guarda el nombre y precio del producto tal como
/// estaban al momento de la venta. Si el producto cambia despues, la venta
/// sigue mostrando lo que realmente se vendio.
/// </summary>
public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }

    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto al momento de la venta. Snapshot historico.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    /// <summary>
    /// Precio unitario al momento de la venta. Incluye IVA. Snapshot historico.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Subtotal de la linea: Quantity * UnitPrice. Incluye IVA.
    /// </summary>
    public decimal Subtotal { get; set; }

    // Navegacion
    public Sale Sale { get; set; } = null!;
}