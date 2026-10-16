namespace Deporcheves.Domain.Entities;

public class Purchase
{
    public int Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    public Supplier Supplier { get; set; } = null!;

    private readonly List<PurchaseItem> _items = new();
    public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Agrega un item a la compra, sube el stock y actualiza el ultimo costo del producto.
    /// </summary>
    public void AddItem(Product product, int quantity, decimal unitCost)
    {
        if (product is null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));
        if (unitCost < 0) throw new ArgumentException("El costo no puede ser negativo.", nameof(unitCost));

        product.IncreaseStock(quantity);

        // Costeo simple: el ultimo costo reemplaza al anterior.
        product.Cost = unitCost;
        product.UpdatedAt = DateTime.UtcNow;

        var item = new PurchaseItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = quantity,
            UnitCost = unitCost,
            Subtotal = unitCost * quantity
        };

        _items.Add(item);
        Total += item.Subtotal;
    }
}