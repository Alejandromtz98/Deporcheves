using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class FixedCostRepository: IFixedCostRepository
{
    private readonly DeporchevesDbContext _context;
    public FixedCostRepository(DeporchevesDbContext context) => _context = context;

    public async Task<IReadOnlyList<FixedCost>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FixedCosts
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }
    public async Task AddAsync(FixedCost cost, CancellationToken cancellationToken = default)
    {
        await _context.FixedCosts.AddAsync(cost,cancellationToken);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}