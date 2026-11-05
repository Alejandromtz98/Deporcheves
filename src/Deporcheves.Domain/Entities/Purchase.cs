namespace Deporcheves.Domain.Entities;

public class Purchase
{
    public int Id { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    // NUEVOS CAMPOS PARA CUENTAS POR PAGAR
    public decimal PaidAmount { get; private set; }
    public decimal Balance => Total - PaidAmount;
    public bool IsFullyPaid => Balance <= 0.01m;

    public Supplier Supplier { get; set; } = null!;

    private readonly List<PurchaseItem> _items = new();
    public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity, decimal unitCost)
    {
        if (product is null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));
        if (unitCost < 0) throw new ArgumentException("El costo no puede ser negativo.", nameof(unitCost));

        // 1. Calculamos el nuevo costo promedio ANTES de que el InventoryService suba el stock
        product.UpdateCostWeightedAverage(quantity, unitCost);
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

    public void RegisterPayment(decimal amount)
    {
        if (amount < 0) throw new ArgumentException("El pago no puede ser negativo.");
        if (amount > Balance) throw new ArgumentException("El pago excede el saldo pendiente.");
        
        PaidAmount += amount;
    }
}