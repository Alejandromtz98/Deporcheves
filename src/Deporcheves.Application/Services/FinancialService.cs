using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Finance;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public class FinancialService : IFinancialService
{
    private readonly ISaleRepository _saleRepo;
    private readonly ICashSessionRepository _cashRepo;
    private readonly IFixedCostRepository _fixedCostRepo;
    private readonly IProductRepository _productRepo;
    private readonly IReceivableRepository _receivableRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IExpenseRepository _expenseRepository; // Usamos el repositorio

    public FinancialService(
        ISaleRepository saleRepo, 
        ICashSessionRepository cashRepo, 
        IFixedCostRepository fixedCostRepo,
        IProductRepository productRepo,
        IReceivableRepository receivableRepository,
        IPurchaseRepository purchaseRepository,
        IExpenseRepository expenseRepository) // Inyectado
    {
        _saleRepo = saleRepo;
        _cashRepo = cashRepo;
        _fixedCostRepo = fixedCostRepo;
        _productRepo = productRepo;
        _receivableRepository = receivableRepository;
        _purchaseRepository = purchaseRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<FinancialDashboardDto> GetDashBoardAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        
        // 1. Gastos en efectivo desde Caja
        var allSessions = await _cashRepo.GetSessionsByDateRangeAsync(startDate, endDate, cancellationToken);
        var cashOpex = allSessions
            .SelectMany(s => s.Movements)
            .Where(m => m.Type == CashMovementType.Expense)
            .Sum(m => m.Amount);

        // 2. Gastos desde la nueva tabla (Renta, Luz, Nómina)
        var dbExpenses = await _expenseRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var dbOpex = dbExpenses
            .Where(e => e.Status == ExpenseStatus.Paid)
            .Sum(e => e.Amount);

        var totalOpex = cashOpex + dbOpex;

        var purchases = await _purchaseRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        decimal totalPaidPurchases = purchases.Sum(p => p.PaidAmount);
        decimal totalFiadoSales = sales.Sum(s => s.AmountFiado);

        decimal totalRevenue = sales.Sum(s => s.Total);
        decimal realCashFlow = (totalRevenue - totalFiadoSales) - totalPaidPurchases - totalOpex;
        
        decimal totalCogs = 0;
        foreach (var sale in sales)
        {
            foreach (var item in sale.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId, cancellationToken);
                if (product != null) totalCogs += (product.Cost * item.Quantity);
            }
        }

        decimal grossProfit = totalRevenue - totalCogs;
        decimal netProfit = grossProfit - totalOpex;
        
        decimal totalReciables = await _receivableRepository.GetTotalPendingBalanceAsync(cancellationToken);
        decimal totalPayables = await _purchaseRepository.GetTotalPendingPayableBalanceAsync(cancellationToken);

        var fixedCosts = await _fixedCostRepo.GetAllActiveAsync(cancellationToken);
        decimal monthlyFixedCosts = fixedCosts.Sum(c => c.MonthlyAmount);
        
        decimal grossMarginRatio = totalRevenue > 0 ? (grossProfit / totalRevenue) : 0;
        decimal breakEvenPoint = grossMarginRatio > 0 ? (monthlyFixedCosts / grossMarginRatio) : 0;

        decimal inventoryValue = await _productRepo.GetTotalInventoryValueAsync(cancellationToken);
        decimal safetyBuffer = monthlyFixedCosts; 
        decimal safeWithdrawal = netProfit - inventoryValue - safetyBuffer;
        if (safeWithdrawal < 0) safeWithdrawal = 0;

        return new FinancialDashboardDto
        {
            PeriodStart = startDate,
            PeriodEnd = endDate,
            TotalRevenue = totalRevenue,
            TotalCogs = totalCogs,
            GrossProfit = grossProfit,
            TotalOpex = totalOpex,
            NetProfit = netProfit,
            MonthlyFixedCosts = monthlyFixedCosts,
            BreakEvenPoint = breakEvenPoint,
            InventoryValue = inventoryValue,
            SafeWithdrawal = safeWithdrawal,
            TotalReceivables = totalReciables,
            TotalPayables = totalPayables,
            RealCashFlow = realCashFlow
        };
    }
}