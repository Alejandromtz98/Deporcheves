using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deporcheves.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ExpensesController : Controller
{
    private readonly IExpenseRepository _repository;

    public ExpensesController(IExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var expenses = await _repository.GetHistoryAsync(cancellationToken);
        
        // Evitamos nulos asegurando que siempre pasemos una lista
        var employees = await _repository.GetActiveEmployeesAsync(cancellationToken);
        ViewBag.Employees = employees ?? new List<Employee>();
        
        return View(expenses);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DateTime date, string category, string description, decimal amount, string paymentMethod, int? employeeId, CancellationToken cancellationToken)
    {
        if (amount > 0 && !string.IsNullOrWhiteSpace(description))
        {
            var expense = new Expense
            {
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                Category = category,
                Description = description,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = ExpenseStatus.Paid,
                EmployeeId = category == "NÓMINA" ? employeeId : null
            };

            await _repository.AddAsync(expense, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
        
        return RedirectToAction(nameof(Index));
    }
}