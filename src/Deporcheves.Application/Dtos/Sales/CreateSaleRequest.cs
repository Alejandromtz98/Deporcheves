using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Dtos.Sales;

public class CreateSaleRequest
{
    public PaymentMethod PaymentMethod { get; set; }
    public List<CreateSaleItemRequest> Items { get; set; } = new();
    
    // NUEVOS CAMPOS PARA EL FIADO
    public int? CustomerId { get; set; }
    public decimal AmountFiado { get; set; } 
}