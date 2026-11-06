using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class Payroll
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Advances { get; set; }
    public decimal Deductions { get; set; }
    public decimal Adjustments { get; set; }
    public decimal TotalCalculated { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // PAGADA, PENDIENTE
    public string? Reference { get; set; }

    public Employee Employee { get; set; } = null!;
}