using Deporcheves.Application.Dtos.Suppliers;

namespace Deporcheves.Application.Services;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<SupplierDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SupplierDto> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<SupplierDto> UpdateAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default);
}