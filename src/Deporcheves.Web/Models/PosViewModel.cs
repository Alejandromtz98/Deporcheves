using Deporcheves.Domain.Entities;

namespace Deporcheves.Web.Models;

public class PosViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<CartItemViewModel> Cart { get; set; } = new();
    public decimal CartTotal => Cart.Sum(i => i.Subtotal);
    public string? SearchTerm { get; set; }
    public string? ErrorMessage { get; set; }
    
    // NUEVOS CAMPOS PARA PAGOS MIXTOS
    public decimal AmountCash { get; set; }
    public decimal AmountTransfer { get; set; }
    public decimal AmountCard { get; set; }
    public decimal AmountFiado { get; set; }

    // Clientes para el fiado
    public List<Customer> Customers { get; set; } = new();
    public int? SelectedCustomerId { get; set; }
}