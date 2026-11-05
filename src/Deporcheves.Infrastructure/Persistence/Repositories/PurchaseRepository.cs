using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DeporchevesDbContext _context;

    public PurchaseRepository(DeporchevesDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetByDateRangeAsync(
        DateTime fromUtc, DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .AsNoTracking()
            .Where(p => p.CreatedAt >= fromUtc && p.CreatedAt < toUtc)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetLastPurchaseNumberForDateAsync(
        DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _context.Purchases
            .AsNoTracking()
            .Where(p => p.CreatedAt >= dayStart && p.CreatedAt < dayEnd)
            .OrderByDescending(p => p.PurchaseNumber)
            .Select(p => p.PurchaseNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Purchase>> GetPendingPayablesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.Supplier)
            .Where(p => p.PaidAmount < p.Total) // Solo las que tienen saldo
            .OrderBy(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Purchase?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<decimal> GetTotalPendingPayableBalanceAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Where(p => p.PaidAmount < p.Total)
            .SumAsync(p => p.Total - p.PaidAmount, cancellationToken);
    }    

    public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        await _context.Purchases.AddAsync(purchase, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}