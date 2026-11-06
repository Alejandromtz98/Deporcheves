using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Deporcheves.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllActiveAsync(cancellationToken);
        return View(products);
    }

    public IActionResult Create() => View(new ProductFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var exists = await _productRepository.ExistsWithSkuAsync(model.Sku, cancellationToken: cancellationToken);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.Sku), "Ya existe un producto con ese SKU.");
            return View(model);
        }

        var product = new Product
        {
            Sku = model.Sku.Trim(),
            Name = model.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            Price = model.Price,
            Cost = model.Cost,
            Stock = model.Stock,
            MinimumStock = model.MinimumStock,
            Unit = model.Unit
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null) return NotFound();

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Cost = product.Cost,
            Stock = product.Stock,
            MinimumStock = product.MinimumStock,
            Unit = product.Unit
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        var product = await _productRepository.GetByIdAsync(model.Id, cancellationToken);
        if (product is null) return NotFound();

        // Validar SKU duplicado (excluyendo el actual)
        if (await _productRepository.ExistsWithSkuAsync(model.Sku, model.Id, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Sku), "Ya existe otro producto con ese SKU.");
            return View(model);
        }

        product.Sku = model.Sku.Trim();
        product.Name = model.Name.Trim();
        product.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        product.Price = model.Price;
        product.Cost = model.Cost;
        product.MinimumStock = model.MinimumStock;
        product.Unit = model.Unit;
        // Nota: No actualizamos el Stock directamente aquí (para eso está el Kardex/Ajustes)

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Index));
    }
}