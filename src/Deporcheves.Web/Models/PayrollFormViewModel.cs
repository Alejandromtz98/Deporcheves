using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Web.Models;

public class PayrollFormViewModel
{
    [Required]
    [Display(Name = "Empleado")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Inicio del Periodo")]
    public DateTime PeriodStart { get; set; } = DateTime.Today.AddDays(-6);

    [Required]
    [Display(Name = "Fin del Periodo")]
    public DateTime PeriodEnd { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Fecha de Pago")]
    public DateTime PaymentDate { get; set; } = DateTime.Today;

    [Display(Name = "Sueldo Base")]
    public decimal BaseSalary { get; set; }

    [Display(Name = "Anticipos")]
    public decimal Advances { get; set; }

    [Display(Name = "Deducciones")]
    public decimal Deductions { get; set; }

    [Display(Name = "Ajustes")]
    public decimal Adjustments { get; set; }

    [Display(Name = "Forma de Pago")]
    public string PaymentMethod { get; set; } = "Efectivo";

    [Display(Name = "Comprobante / Referencia")]
    public string? Reference { get; set; }
}