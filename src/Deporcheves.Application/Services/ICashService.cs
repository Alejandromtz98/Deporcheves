using Deporcheves.Application.Dtos.Cash;

namespace Deporcheves.Application.Services;

public interface ICashService
{
    Task<CashReportDto> OpenSessionAsync(decimal openingCash, CancellationToken cancellationToken = default);
    Task<CashReportDto> GetActiveSessionReportAsync(CancellationToken cancellationToken = default);
    Task RegisterMovementAsync(CashMovementRequest request, CancellationToken cancellationToken = default);
    Task<CashReportDto> CloseSessionAsync(decimal closingCash, CancellationToken cancellationToken = default);
}