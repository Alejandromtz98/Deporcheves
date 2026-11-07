using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}