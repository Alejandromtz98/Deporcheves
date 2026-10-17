using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface ICashSessionRepository
{
    Task<CashSession?> GetActiveSessionAsync(CancellationToken cancellationToken = default);
    Task<CashSession?> GetByIdWithMovementsAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(CashSession session, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}