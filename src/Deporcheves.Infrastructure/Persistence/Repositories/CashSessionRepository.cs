using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class CashSessionRepository : ICashSessionRepository
{
    private readonly DeporchevesDbContext _context;

    public CashSessionRepository(DeporchevesDbContext context) => _context = context;

    public async Task<CashSession?> GetActiveSessionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CashSessions
            .Include(c => c.Movements)
            .FirstOrDefaultAsync(c => c.ClosedAt == null, cancellationToken);
    }

    public async Task<CashSession?> GetByIdWithMovementsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CashSessions
            .Include(c => c.Movements)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(CashSession session, CancellationToken cancellationToken = default)
    {
        await _context.CashSessions.AddAsync(session, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}