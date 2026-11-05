namespace Deporcheves.Application.Dtos.Sales;

public class CreateSaleItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int? CustomerId { get; set; }
    public decimal AmountFiado { get; set; } // Cuánto se va a fiar
}