using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public interface IReceivableService
{
    Task<IReadOnlyList<AccountsReceivable>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task RegisterPaymentAsync(int id, decimal amount, CancellationToken cancellationToken = default);
}