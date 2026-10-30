namespace Deporcheves.Application.Dtos.Finance;

public class FinancialDashboardDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    public decimal TotalRevenue { get; set; }
    public decimal TotalCogs { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal TotalOpex { get; set; }
    public decimal NetProfit { get; set; }

    public decimal GrossMarginPercentage => TotalRevenue > 0 ? (GrossProfit / TotalRevenue) * 100 : 0;
    
    public decimal MonthlyFixedCosts { get; set; }
    public decimal BreakEvenPoint { get; set; }
    
    public decimal InventoryValue { get; set; }
    public decimal SafeWithdrawal { get; set; }
}