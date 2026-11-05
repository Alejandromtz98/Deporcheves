using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class AccountsReceivable
{
    public int Id { get; private set; }
    public int SaleId { get; private set; }
    public int CustomerId { get; private set; }
    
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal Balance { get; private set; }
    
    public ReceivableStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public Sale Sale { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;

    protected AccountsReceivable() { }

    public AccountsReceivable(Sale sale, int customerId, decimal totalAmount)
    {
        if (totalAmount <= 0) throw new ArgumentException("El monto del fiado debe ser mayor a cero.");

        Sale = sale;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        PaidAmount = 0;
        Balance = totalAmount;
        Status = ReceivableStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void RegisterPayment(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El pago debe ser mayor a cero.");
        if (amount > Balance) throw new ArgumentException("El pago no puede exceder el saldo pendiente.");
        if (Status == ReceivableStatus.Paid) throw new InvalidOperationException("Esta cuenta ya está pagada.");

        PaidAmount += amount;
        Balance -= amount;

        if (Balance <= 0.01m)
        {
            Balance = 0;
            Status = ReceivableStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }
        else
        {
            Status = ReceivableStatus.PartiallyPaid;
        }
    }
}