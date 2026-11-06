using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Domain.Enums;

public enum CashMovementType
{
    [Display(Name = "Gasto")]
    Expense = 1,
    
    [Display(Name = "Retiro")]
    Withdrawal = 2,
    
    [Display(Name = "Ingreso (Cobro)")]
    Income = 3
}