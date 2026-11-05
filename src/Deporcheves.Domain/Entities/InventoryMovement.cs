using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class InventoryMovement
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public InventoryMovementType Type { get; private set; }
    public int Quantity { get; private set; } // Positivo = Entrada, Negativo = Salida
    public decimal UnitCost { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Reference { get; private set; } = string.Empty; // ID de Venta, Compra o Ajuste
    public string? Reason { get; private set; }   // Motivo (ej. "Merma por rotura")

    public Product Product { get; private set; } = null!;

    protected InventoryMovement() { Reference = null!; }

    public InventoryMovement(int productId, InventoryMovementType type, int quantity, decimal unitCost, string reference, string? reason = null)
    {
        if(quantity == 0)
            throw new ArgumentException("La cantidad no puede ser 0.", nameof(quantity));

        ProductId = productId;
        Type = type;
        Quantity = quantity;
        UnitCost = unitCost;
        Reference = reference;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }
}