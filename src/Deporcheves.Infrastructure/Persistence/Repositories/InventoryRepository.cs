using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly DeporchevesDbContext _context;
    public InventoryRepository(DeporchevesDbContext context) => _context = context;

    public async Task AddMovementAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
        => await _context.InventoryMovements.AddAsync(movement, cancellationToken);

    public async Task<InventoryCount?> GetCountByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.InventoryCounts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddCountAsync(InventoryCount count, CancellationToken cancellationToken = default)
        => await _context.InventoryCounts.AddAsync(count, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}