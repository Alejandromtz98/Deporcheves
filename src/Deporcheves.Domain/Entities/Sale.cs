using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    
    // PAGOS MIXTOS
    public decimal AmountCash { get; set; }
    public decimal AmountTransfer { get; set; }
    public decimal AmountCard { get; set; }
    public decimal AmountFiado { get; set; }

    // Relaciones
    public int? CashSessionId { get; set; }
    public CashSession? CashSession { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity, decimal discount = 0)
    {
        if (product is null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));
        if (discount < 0) throw new ArgumentException("El descuento no puede ser negativo.", nameof(discount));

        var item = new SaleItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = quantity,
            UnitPrice = product.Price,
            Discount = discount
        };

        _items.Add(item);
        Total += item.Subtotal; // Suma el subtotal con el descuento aplicado
    }
}