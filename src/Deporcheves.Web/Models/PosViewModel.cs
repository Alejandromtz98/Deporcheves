using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Web.Models;

public class PosViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<CartItemViewModel> Cart { get; set; } = new();
    public decimal CartTotal => Cart.Sum(i => i.Subtotal);
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ErrorMessage { get; set; }
    public string? SearchTerm { get; set; }
    public int LowStockCount => Products.Count(p => p.IsBelowMinimumStock());
}