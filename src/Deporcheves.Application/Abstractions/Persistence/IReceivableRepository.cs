using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface IReceivableRepository
{
    Task<AccountsReceivable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountsReceivable>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AccountsReceivable receivable, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}