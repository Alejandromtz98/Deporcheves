using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface IFixedCostRepository
{
    Task<IReadOnlyList<FixedCost>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(FixedCost cost, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}