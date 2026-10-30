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

    public FinancialService(
        ISaleRepository saleRepo, 
        ICashSessionRepository cashRepo, 
        IFixedCostRepository fixedCostRepo,
        IProductRepository productRepo)
    {
        _saleRepo = saleRepo;
        _cashRepo = cashRepo;
        _fixedCostRepo = fixedCostRepo;
        _productRepo = productRepo;
    }

    public async Task<FinancialDashboardDto> GetDashBoardAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // 1. Obtener ventas y gastos del periodo
        // Nota: Asumimos que GetByDateRangeAsync trae los items (Include)
        var sales = await _saleRepo.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        
        // Los gastos operativos (OPEX) están en los movimientos de caja. 
        // Para simplificar, sumaremos los gastos de todas las sesiones en el rango de fechas.
        var allSessions = await _cashRepo.GetSessionsByDateRangeAsync(startDate, endDate, cancellationToken);
        var totalOpex = allSessions
            .SelectMany(s => s.Movements)
            .Where(m => m.Type == CashMovementType.Expense)
            .Sum(m => m.Amount);

        // 2. Calcular Ingresos y COGS
        decimal totalRevenue = sales.Sum(s => s.Total);
        
        // COGS: Cruzamos los items vendidos con el costo actual del producto
        decimal totalCogs = 0;
        foreach (var sale in sales)
        {
            foreach (var item in sale.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId, cancellationToken);
                if (product != null)
                {
                    totalCogs += (product.Cost * item.Quantity);
                }
            }
        }

        // 3. Calcular Utilidades
        decimal grossProfit = totalRevenue - totalCogs;
        decimal netProfit = grossProfit - totalOpex;

        // 4. Calcular Punto de Equilibrio
        var fixedCosts = await _fixedCostRepo.GetAllActiveAsync(cancellationToken);
        decimal monthlyFixedCosts = fixedCosts.Sum(c => c.MonthlyAmount);
        
        decimal grossMarginRatio = totalRevenue > 0 ? (grossProfit / totalRevenue) : 0;
        // Evitar división por cero
        decimal breakEvenPoint = grossMarginRatio > 0 
            ? (monthlyFixedCosts / grossMarginRatio) 
            : 0;

        // 5. Calcular Retiro Seguro (FCF)
        decimal inventoryValue = await _productRepo.GetTotalInventoryValueAsync(cancellationToken);
        
        // Colchón de seguridad: 1 mes de costos fijos
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
            SafeWithdrawal = safeWithdrawal
        };
    }
}