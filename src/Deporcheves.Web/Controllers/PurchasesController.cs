using System.Text.Json;
using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Purchases;
using Deporcheves.Application.Services;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Deporcheves.Web.Controllers;

[Authorize(Roles = "Admin")]
public class PurchasesController : Controller
{
    private const string SupplierSessionKey = "PurchaseCart.SupplierId";
    private const string ItemsSessionKey = "PurchaseCart.Items";

    private readonly IPurchaseService _purchaseService;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierService _supplierService;

    public PurchasesController(
        IPurchaseService purchaseService,
        IProductRepository productRepository,
        ISupplierService supplierService)
    {
        _purchaseService = purchaseService;
        _productRepository = productRepository;
        _supplierService = supplierService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var purchases = await _purchaseService.GetPurchasesByDateAsync(DateTime.UtcNow, cancellationToken);
        return View(purchases);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        return View(await BuildFormAsync(null, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(int productId, int quantity, decimal unitCost, CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            return View("Create", await BuildFormAsync("La cantidad debe ser mayor a cero.", cancellationToken));

        if (unitCost < 0)
            return View("Create", await BuildFormAsync("El costo no puede ser negativo.", cancellationToken));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null || !product.IsActive)
            return View("Create", await BuildFormAsync("Producto no valido.", cancellationToken));

        var cart = GetCart();
        var existing = cart.FirstOrDefault(i => i.ProductId == productId);

        if (existing is null)
        {
            cart.Add(new PurchaseCartItemViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = quantity,
                UnitCost = unitCost
            });
        }
        else
        {
            existing.Quantity += quantity;
            existing.UnitCost = unitCost;
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Create));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(cart);
        return RedirectToAction(nameof(Create));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearCart()
    {
        SaveCart(new List<PurchaseCartItemViewModel>());
        return RedirectToAction(nameof(Create));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int supplierId, CancellationToken cancellationToken)
    {
        var cart = GetCart();
        if (cart.Count == 0)
            return View("Create", await BuildFormAsync("La compra esta vacia.", cancellationToken));

        if (supplierId <= 0)
            return View("Create", await BuildFormAsync("Selecciona un proveedor antes de confirmar.", cancellationToken));

        var request = new CreatePurchaseRequest
        {
            SupplierId = supplierId,
            Items = cart.Select(i => new CreatePurchaseItemRequest
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost
            }).ToList()
        };

        try
        {
            var purchase = await _purchaseService.RegisterPurchaseAsync(request, cancellationToken);
            ClearSession();
            return RedirectToAction(nameof(Details), new { id = purchase.Id });
        }
        catch (InvalidOperationException ex)
        {
            return View("Create", await BuildFormAsync(ex.Message, cancellationToken));
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseService.GetPurchaseByIdAsync(id, cancellationToken);
        if (purchase is null) return NotFound();
        return View(purchase);
    }

    // ------------------------------------------------------------
    // Privados
    // ------------------------------------------------------------

    private async Task<PurchaseFormViewModel> BuildFormAsync(string? error, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierService.GetAllActiveAsync(cancellationToken);
        var products = await _productRepository.GetAllActiveAsync(cancellationToken);

        return new PurchaseFormViewModel
        {
            Suppliers = suppliers.ToList(),
            Products = products.ToList(),
            Cart = GetCart(),
            ErrorMessage = error
        };
    }

    private List<PurchaseCartItemViewModel> GetCart()
    {
        var json = HttpContext.Session.GetString(ItemsSessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<PurchaseCartItemViewModel>()
            : JsonSerializer.Deserialize<List<PurchaseCartItemViewModel>>(json) ?? new List<PurchaseCartItemViewModel>();
    }

    private void SaveCart(List<PurchaseCartItemViewModel> cart)
        => HttpContext.Session.SetString(ItemsSessionKey, JsonSerializer.Serialize(cart));

    private void ClearSession()
    {
        HttpContext.Session.Remove(ItemsSessionKey);
    }
}