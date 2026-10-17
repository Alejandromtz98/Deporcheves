using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Supplier>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsWithNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

    void Update(Supplier supplier);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}