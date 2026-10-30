using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public class FixedCostService : IFixedCostService
{
    private readonly IFixedCostRepository _fixedCostRepository;

    public FixedCostService(IFixedCostRepository fixedCostRepository) => _fixedCostRepository = fixedCostRepository;

    public async Task<IReadOnlyList<FixedCost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
            return await _fixedCostRepository.GetAllActiveAsync(cancellationToken);
    }
    public async Task CreateAsync(string name, decimal amount, CancellationToken cancellationToken = default)
    {
        var cost = new FixedCost(name, amount);
        await _fixedCostRepository.AddAsync(cost, cancellationToken);
        await _fixedCostRepository.SaveChangesAsync(cancellationToken);
    }
}