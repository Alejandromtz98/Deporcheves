using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deporcheves.Web.Controllers;

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
}