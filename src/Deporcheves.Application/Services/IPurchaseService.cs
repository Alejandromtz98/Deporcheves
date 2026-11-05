using Deporcheves.Application.Dtos.Purchases;

namespace Deporcheves.Application.Services;

public interface IPurchaseService
{
    Task<PurchaseDto> RegisterPurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseDto>> GetPurchasesByDateAsync(DateTime dateUtc, CancellationToken cancellationToken = default);
    Task<PurchaseDto?> GetPurchaseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseDto>> GetPendingPayablesAsync(CancellationToken cancellationToken = default);
    Task RegisterPaymentAsync(int purchaseId, decimal amount, CancellationToken cancellationToken = default);
}