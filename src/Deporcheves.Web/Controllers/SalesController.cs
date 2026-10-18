using System.Text.Json;
using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Sales;
using Deporcheves.Application.Services;
using Deporcheves.Domain.Enums;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Deporcheves.Web.Controllers;

public class SalesController : Controller
{
    private const string CartSessionKey = "PosCart";
    private readonly ISaleService _saleService;
    private readonly IProductRepository _productRepository;

    public SalesController(ISaleService saleService, IProductRepository productRepository)
    {
        _saleService = saleService;
        _productRepository = productRepository;
    }

   public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken)
    {
        return View(await BuildPosViewModelAsync(null, cancellationToken, search));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(int productId, int quantity, CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            return View("Index", await BuildPosViewModelAsync("La cantidad debe ser mayor a cero.", cancellationToken));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null || !product.IsActive)
            return View("Index", await BuildPosViewModelAsync("Producto no valido.", cancellationToken));

        var cart = GetCart();
        var existing = cart.FirstOrDefault(i => i.ProductId == productId);
        var newQuantity = (existing?.Quantity ?? 0) + quantity;

        if (newQuantity > product.Stock)
            return View("Index", await BuildPosViewModelAsync($"Stock insuficiente. Disponible: {product.Stock}.", cancellationToken));

        if (existing is null)
        {
            cart.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity = newQuantity;
        }

        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearCart()
    {
        SaveCart(new List<CartItemViewModel>());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(PaymentMethod paymentMethod, CancellationToken cancellationToken)
    {
        var cart = GetCart();
        if (cart.Count == 0)
            return View("Index", await BuildPosViewModelAsync("El ticket esta vacio.", cancellationToken));

        var userIdstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdstring) || !int.TryParse(userIdstring, out var userId))
            return View("Index", await BuildPosViewModelAsync("Error en la sesion del usuario", cancellationToken));

        var request = new CreateSaleRequest
        {
            PaymentMethod = paymentMethod,
            Items = cart.Select(i => new CreateSaleItemRequest
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        try
        {
            var sale = await _saleService.RegisterSaleAsync(request, userId, cancellationToken);
            SaveCart(new List<CartItemViewModel>());
            return RedirectToAction(nameof(Details), new { id = sale.Id });
        }
        catch (InvalidOperationException ex)
        {
            return View("Index", await BuildPosViewModelAsync(ex.Message, cancellationToken));
        }
    }

    public async Task<IActionResult> Daily(CancellationToken cancellationToken)
    {
        var sales = await _saleService.GetSalesByDateAsync(DateTime.UtcNow, cancellationToken);
        return View(sales);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var sale = await _saleService.GetSaleByIdAsync(id, cancellationToken);
        if (sale is null) return NotFound();
        return View(sale);
    }

    // ------------------------------------------------------------
    // Privados
    // ------------------------------------------------------------

    private async Task<PosViewModel> BuildPosViewModelAsync(
        string? error,
        CancellationToken cancellationToken,
        string? searchTerm = null)
    {
        var products = string.IsNullOrWhiteSpace(searchTerm)
            ? await _productRepository.GetAllActiveAsync(cancellationToken)
            : await _productRepository.SearchAsync(searchTerm, cancellationToken);

        return new PosViewModel
        {
            Products = products.ToList(),
            Cart = GetCart(),
            ErrorMessage = error,
            SearchTerm = searchTerm
        };
    }

    private List<CartItemViewModel> GetCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        return string.IsNullOrEmpty(json)
            ? new List<CartItemViewModel>()
            : JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>();
    }

    private void SaveCart(List<CartItemViewModel> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }
}