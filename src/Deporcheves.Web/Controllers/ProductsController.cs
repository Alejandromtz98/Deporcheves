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

    public async Task<IActionResult> Index(string? search, string? sortOrder, CancellationToken cancellationToken)
    {
        // 1. Traer todos los productos activos
        var products = await _productRepository.GetAllActiveAsync(cancellationToken);

        // 2. Filtrar por búsqueda
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            products = products.Where(p => p.Name.ToLower().Contains(term) || p.Sku.ToLower().Contains(term)).ToList();
        }

        // 3. Ordenamiento
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
        ViewData["StockSortParm"] = sortOrder == "stock" ? "stock_desc" : "stock";

        products = sortOrder switch
        {
            "name_desc" => products.OrderByDescending(p => p.Name).ToList(),
            "price" => products.OrderBy(p => p.Price).ToList(),
            "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
            "stock" => products.OrderBy(p => p.Stock).ToList(),
            "stock_desc" => products.OrderByDescending(p => p.Stock).ToList(),
            _ => products.OrderBy(p => p.Name).ToList(), // Por defecto orden alfabético
        };

        ViewData["CurrentFilter"] = search;
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