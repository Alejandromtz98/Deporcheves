using Deporcheves.Application.Dtos.Suppliers;
using Deporcheves.Application.Services;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Deporcheves.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SuppliersController : Controller
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var suppliers = await _supplierService.GetAllActiveAsync(cancellationToken);
        return View(suppliers);
    }

    public IActionResult Create() => View(new SupplierFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _supplierService.CreateAsync(new CreateSupplierRequest
            {
                Name = model.Name,
                ContactName = model.ContactName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address
            }, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.GetByIdAsync(id, cancellationToken);
        if (supplier is null) return NotFound();

        return View(new SupplierFormViewModel
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactName = supplier.ContactName,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SupplierFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _supplierService.UpdateAsync(new UpdateSupplierRequest
            {
                Id = model.Id,
                Name = model.Name,
                ContactName = model.ContactName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address
            }, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}