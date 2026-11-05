using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class ReceivableRepository : IReceivableRepository
{
    private readonly DeporchevesDbContext _context;
    public ReceivableRepository(DeporchevesDbContext context) => _context = context;

    public async Task<AccountsReceivable?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.AccountsReceivables.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<AccountsReceivable>> GetPendingAsync(CancellationToken cancellationToken = default)
        => await _context.AccountsReceivables
            .Include(a => a.Customer)
            .Where(a => a.Status == ReceivableStatus.Pending || a.Status == ReceivableStatus.PartiallyPaid)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    public async Task<decimal> GetTotalPendingBalanceAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AccountsReceivables
            .Where(a => a.Status == ReceivableStatus.Pending || a.Status == ReceivableStatus.PartiallyPaid)
            .SumAsync(a => a.Balance, cancellationToken);
    }

    public async Task AddAsync(AccountsReceivable receivable, CancellationToken cancellationToken = default)
        => await _context.AccountsReceivables.AddAsync(receivable, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}