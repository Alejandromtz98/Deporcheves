using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

/// <summary>
/// Venta registrada en el punto de venta. Es la raiz del agregado Sale.
/// Una venta es inmutable: una vez creada, no se modifica ni se elimina.
/// Los cambios al catalogo de productos no afectan ventas historicas.
/// </summary>
public class Sale
{
    public int Id { get; set; }

    /// <summary>
    /// Numero legible unico de la venta. Formato: V-YYYYMMDD-NNNN.
    /// Lo asigna la capa de aplicacion al registrar la venta.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Total de la venta. Incluye IVA. Es la suma de los subtotales de los items.
    /// </summary>
    public decimal Total { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    // FK al turno de caja. Nullable para no romper ventas de prueba anteriores.
    public int? CashSessionId { get; set; }
    public CashSession? CashSession { get; set; }
    // FK al usuario que registro la venta.
    public int? UserId { get; set; }
    public User? User { get; set; }

    private readonly List<SaleItem> _items = new();

    /// <summary>
    /// Items de la venta. Expuesto como solo lectura: solo se puede modificar
    /// a traves de AddItem.
    /// </summary>
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Agrega un item a la venta. Disminuye el stock del producto y recalcula el total.
    /// Lanza excepcion si el producto no tiene stock suficiente.
    /// </summary>
    public void AddItem(Product product, int quantity)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));

        // DecreaseStock valida que quantity > 0 y que haya stock suficiente.
        product.DecreaseStock(quantity);

        var item = new SaleItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = quantity,
            UnitPrice = product.Price,
            Subtotal = product.Price * quantity
        };

        _items.Add(item);
        Total += item.Subtotal;
    }
}