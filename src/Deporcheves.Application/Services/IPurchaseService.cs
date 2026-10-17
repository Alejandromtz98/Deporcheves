using Deporcheves.Application.Dtos.Purchases;

namespace Deporcheves.Application.Services;

public interface IPurchaseService
{
    Task<PurchaseDto> RegisterPurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseDto>> GetPurchasesByDateAsync(DateTime dateUtc, CancellationToken cancellationToken = default);
    Task<PurchaseDto?> GetPurchaseByIdAsync(int id, CancellationToken cancellationToken = default);
}