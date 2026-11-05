namespace Deporcheves.Domain.Entities;

public class InventoryCount
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int TheoreticalQuantity { get; private set; }
    public int PhysicalQuantity { get; private set; }
    public int Difference { get; private set; }
    public string Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsAdjusted { get; private set; }

    public Product Product { get; private set; } = null!;

    protected InventoryCount() { Reason = null!; }

    public InventoryCount(int productId, int theoreticalQuantity, int physicalQuantity, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("El motivo del conteo es obligatorio.", nameof(reason));

        ProductId = productId;
        TheoreticalQuantity = theoreticalQuantity;
        PhysicalQuantity = physicalQuantity;
        Difference = theoreticalQuantity - physicalQuantity;
        Reason = reason.Trim();
        CreatedAt = DateTime.UtcNow;
        IsAdjusted = false;
    }

    public void MarkAsAdjusted()
    {
        if (IsAdjusted) throw new InvalidOperationException("Este conteo ya fue ajustado.");
        IsAdjusted = true;
    }
}