namespace Deporcheves.Application.Dtos.Cash;

public class CashReportDto
{
    public int SessionId { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningCash { get; set; }
    
    public decimal CashSales { get; set; }
    public decimal CardSales { get; set; }
    public decimal TransferSales { get; set; }
    
    public decimal TotalExpenses { get; set; }
    public decimal TotalWithdrawals { get; set; }
    
    public decimal ExpectedCash { get; set; }
    public decimal? ClosingCash { get; set; }
    public decimal? Difference { get; set; }
    
    public bool IsClosed => ClosedAt.HasValue;
}