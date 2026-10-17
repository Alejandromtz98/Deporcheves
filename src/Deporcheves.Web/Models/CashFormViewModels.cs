using System.ComponentModel.DataAnnotations;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Web.Models;

public class OpenCashViewModel
{
    [Range(0, 999999, ErrorMessage = "El fondo inicial no puede ser negativo.")]
    [Display(Name = "Fondo inicial en caja")]
    public decimal OpeningCash { get; set; }
}

public class CloseCashViewModel
{
    [Range(0, 999999, ErrorMessage = "El efectivo contado no puede ser negativo.")]
    [Display(Name = "Efectivo contado fisicamente")]
    public decimal ClosingCash { get; set; }
}

public class CashMovementViewModel
{
    public CashMovementType Type { get; set; }
    
    [Range(0.01, 999999, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(300)]
    public string Reason { get; set; } = string.Empty;
}