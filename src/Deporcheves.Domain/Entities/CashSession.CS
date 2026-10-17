 using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

public class CashSession
{
    public int Id { get; set; }
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    
    public decimal OpeningCash { get; set; }
    
    // Campos que se llenan al hacer el Cierre Z
    public decimal? ExpectedCash { get; set; }
    public decimal? ClosingCash { get; set; }
    public decimal? Difference { get; set; }

    public bool IsClosed => ClosedAt.HasValue;

    private readonly List<CashMovement> _movements = new();
    public IReadOnlyCollection<CashMovement> Movements => _movements.AsReadOnly();

    public void RegisterMovement(CashMovementType type, decimal amount, string reason)
    {
        if (IsClosed) throw new InvalidOperationException("No se pueden registrar movimientos en una caja cerrada.");
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.", nameof(amount));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("El motivo es obligatorio.", nameof(reason));

        _movements.Add(new CashMovement
        {
            Type = type,
            Amount = amount,
            Reason = reason.Trim()
        });
    }

    public void Close(decimal expectedCash, decimal closingCash)
    {
        if (IsClosed) throw new InvalidOperationException("La caja ya esta cerrada.");
        if (closingCash < 0) throw new ArgumentException("El efectivo contado no puede ser negativo.");

        ClosedAt = DateTime.UtcNow;
        ExpectedCash = expectedCash;
        ClosingCash = closingCash;
        Difference = closingCash - expectedCash;
    }
}