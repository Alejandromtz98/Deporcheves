namespace Deporcheves.Domain.Entities;

public class FinancialSnapshot
{
    public int Id{get; private set;}
    public DateTime PeriodStart{get; private set;}
    public DateTime PeriodEnd{get; private set;}

    public decimal TotalRevenue {get; private set;}
    public decimal TotalCogs {get; private set;} // Costos de venta
    public decimal TotalExpenses {get; private set;} // Costos operativos

    public decimal GrossProfit => TotalRevenue - TotalCogs;
    public decimal NetProfit => GrossProfit - TotalExpenses;

    protected FinancialSnapshot(){}

    public FinancialSnapshot(DateTime start, DateTime end, decimal revenue, decimal cogs, decimal expenses)
    {
        if(end <= start)
            throw new ArgumentException("La fecha fin de ser posterior a la fecha de inicio");

        PeriodStart = start;
        PeriodEnd = end;
        TotalRevenue = revenue;
        TotalCogs = cogs;
        TotalExpenses = expenses;
    }

}