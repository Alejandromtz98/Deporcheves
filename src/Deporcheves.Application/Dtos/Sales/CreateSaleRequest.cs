namespace Deporcheves.Application.Dtos.Sales;

public class CreateSaleRequest
{
    public List<CreateSaleItemRequest> Items { get; set; } = new();
    
    // PAGOS MIXTOS (Reemplazan al PaymentMethod único)
    public decimal AmountCash { get; set; }
    public decimal AmountTransfer { get; set; }
    public decimal AmountCard { get; set; }
    public decimal AmountFiado { get; set; }

    // Cliente para el fiado
    public int? CustomerId { get; set; }
}