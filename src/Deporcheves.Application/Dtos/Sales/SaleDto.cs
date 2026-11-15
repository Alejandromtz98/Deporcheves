namespace Deporcheves.Application.Dtos.Sales;

public class SaleDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    
    // NUEVOS CAMPOS DE PAGO
    public decimal AmountCash { get; set; }
    public decimal AmountTransfer { get; set; }
    public decimal AmountCard { get; set; }
    public decimal AmountFiado { get; set; }

    public List<SaleItemDto> Items { get; set; } = new();
}