using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DeporchevesDbContext _context;

    public SaleRepository(DeporchevesDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(
        DateTime fromUtc, DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.CreatedAt >= fromUtc && s.CreatedAt < toUtc)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetLastSaleNumberForDateAsync(
        DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _context.Sales
            .AsNoTracking()
            .Where(s => s.CreatedAt >= dayStart && s.CreatedAt < dayEnd)
            .OrderByDescending(s => s.SaleNumber)
            .Select(s => s.SaleNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}