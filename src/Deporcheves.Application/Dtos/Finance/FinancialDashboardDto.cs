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
    public decimal TotalReceivables { get; set; } // Dinero que nos deben (CxC)
    public decimal TotalPayables { get; set; }    // Dinero que debemos (CxP)
    public decimal RealCashFlow { get; set; }     // Efectivo real generado en el periodo
    public int TotalOperations { get; set; }
    public decimal AverageTicket { get; set; }
    public int TotalUnitsSold { get; set; }
    public int ProductsWithoutCost { get; set; }
    public decimal InitialCapital { get; set; }
    public decimal ReturnOnInvestment => InitialCapital > 0 ? (NetProfit / InitialCapital) * 100 : 0;
}