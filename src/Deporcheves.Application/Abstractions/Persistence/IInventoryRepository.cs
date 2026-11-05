using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface IInventoryRepository
{
    Task AddMovementAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
    Task<InventoryCount?> GetCountByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddCountAsync(InventoryCount count, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}