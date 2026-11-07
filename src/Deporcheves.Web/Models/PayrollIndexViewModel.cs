using Deporcheves.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Deporcheves.Web.Models;

public class PayrollIndexViewModel
{
    public IReadOnlyList<Payroll> Payrolls { get; set; } = new List<Payroll>();
    public PayrollFormViewModel Form { get; set; } = new PayrollFormViewModel();
    public SelectList? Employees { get; set; }
}