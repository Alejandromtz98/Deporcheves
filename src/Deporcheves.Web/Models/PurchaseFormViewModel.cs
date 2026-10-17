using Deporcheves.Application.Dtos.Suppliers;

namespace Deporcheves.Web.Models;

public class PurchaseFormViewModel
{
    public List<SupplierDto> Suppliers { get; set; } = new();
    public int? SelectedSupplierId { get; set; }
    public string? SelectedSupplierName { get; set; }
    public List<Deporcheves.Domain.Entities.Product> Products { get; set; } = new();
    public List<PurchaseCartItemViewModel> Cart { get; set; } = new();
    public decimal CartTotal => Cart.Sum(i => i.Subtotal);
    public string? ErrorMessage { get; set; }
}