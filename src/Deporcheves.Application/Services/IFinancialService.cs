using Deporcheves.Application.Dtos.Finance;

namespace Deporcheves.Application.Services;

public interface IFinancialService
{
    Task<FinancialDashboardDto> GetDashBoardAsync(DateTime startDate, DateTime END, CancellationToken cancellationToken = default);
}