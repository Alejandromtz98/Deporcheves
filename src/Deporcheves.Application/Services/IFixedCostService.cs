using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public interface IFixedCostService
{
    Task<IReadOnlyList<FixedCost>> GetAllAsync(CancellationToken cancellationToken = default);
    Task CreateAsync(string name, decimal amoun, CancellationToken cancellationToken = default);
}