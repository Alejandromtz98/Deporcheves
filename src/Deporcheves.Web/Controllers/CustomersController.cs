using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deporcheves.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        // Usamos el buscador con un término vacío para traer los primeros 50 activos
        var customers = await _repository.SearchAsync("", cancellationToken);
        return View(customers);
    }

    public IActionResult Create() => View(new CustomerFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var customer = new Customer
        {
            Name = model.Name.Trim(),
            Phone = model.Phone?.Trim()
        };

        await _repository.AddAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Index));
    }
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return NotFound();

        return View(new CustomerFormViewModel 
        { 
            Id = customer.Id, // Asignamos el ID
            Name = customer.Name, 
            Phone = customer.Phone 
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CustomerFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var customer = await _repository.GetByIdAsync(model.Id, cancellationToken); // Usamos model.Id
        if (customer is null) return NotFound();

        customer.Name = model.Name.Trim();
        customer.Phone = model.Phone?.Trim();
        
        await _repository.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}