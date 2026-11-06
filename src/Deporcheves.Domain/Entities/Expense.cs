using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class Expense
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public ExpenseStatus Status { get; set; }
    public string? Reference { get; set; }
    public string? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}