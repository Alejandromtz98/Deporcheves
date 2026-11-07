using Deporcheves.Domain.Entities;
using Deporcheves.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Web.Controllers;

[Authorize(Roles = "Admin")]
public class EmployeesController : Controller
{
    private readonly DeporchevesDbContext _context;

    public EmployeesController(DeporchevesDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employees = await _context.Employees.AsNoTracking().ToListAsync(cancellationToken);
        return View(employees);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, decimal baseWeeklySalary, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            _context.Employees.Add(new Employee { Name = name.Trim(), BaseWeeklySalary = baseWeeklySalary });
            await _context.SaveChangesAsync(cancellationToken);
        }
        return RedirectToAction(nameof(Index));
    }
}