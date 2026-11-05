using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public interface IInventoryService
{
     Task RegisterMovementAsync(Product product, InventoryMovementType type, int quantity, decimal unitCost, string reference, string? reason = null, CancellationToken cancellationToken = default);
     Task ApplyAdjustmentAsync(int counttId, string authorizedBy, CancellationToken cancellationToken = default);
}