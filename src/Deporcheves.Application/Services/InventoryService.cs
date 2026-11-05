using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IProductRepository _productRepo;

    public InventoryService(IInventoryRepository inventoryRepo, IProductRepository productRepo)
    {
        _inventoryRepo = inventoryRepo;
        _productRepo = productRepo;
    }

    public async Task RegisterMovementAsync(Product product, InventoryMovementType type, int quantity, decimal unitCost, string reference, string? reason = null, CancellationToken cancellationToken = default)
    {
        // quantity > 0 para entradas, quantity < 0 para salidas.
        // Modificamos el Stock del producto (la suma de movimientos debe coincidir)
        if (quantity > 0) product.IncreaseStock(quantity);
        else product.DecreaseStock(Math.Abs(quantity));

        var movement = new InventoryMovement(product.Id, type, quantity, unitCost, reference, reason);
        await _inventoryRepo.AddMovementAsync(movement, cancellationToken);
    }

    public async Task ApplyAdjustmentAsync(int countId, string authorizedBy, CancellationToken cancellationToken = default)
    {
        var count = await _inventoryRepo.GetCountByIdAsync(countId, cancellationToken)
            ?? throw new InvalidOperationException("Conteo no encontrado.");

        if (count.IsAdjusted) throw new InvalidOperationException("Este conteo ya fue ajustado.");

        var product = await _productRepo.GetByIdAsync(count.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("Producto no encontrado.");

        if (count.Difference != 0)
        {
            // Si Theoretical = 10 y Physical = 8, Difference = 2 (salida de 2)
            int quantityToAdjust = count.PhysicalQuantity - count.TheoreticalQuantity; 
            await RegisterMovementAsync(product, InventoryMovementType.Adjustment, quantityToAdjust, product.Cost, count.Id.ToString(), $"Ajuste autorizado por {authorizedBy}: {count.Reason}", cancellationToken);
        }

        count.MarkAsAdjusted();
        await _inventoryRepo.SaveChangesAsync(cancellationToken);
    }
}