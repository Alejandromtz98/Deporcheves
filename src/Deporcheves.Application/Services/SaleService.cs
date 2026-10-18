using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Sales;
using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICashSessionRepository  _cashSessionRepository;

    public SaleService(ISaleRepository saleRepository, IProductRepository productRepository, ICashSessionRepository cashSessionRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _cashSessionRepository = cashSessionRepository;
    }

    public async Task<SaleDto> RegisterSaleAsync(CreateSaleRequest request, int userId, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La venta debe contener al menos un item.");

        var nowUtc = DateTime.UtcNow;
        var saleNumber = await GenerateSaleNumberAsync(nowUtc, cancellationToken);

        var activeSession = await _cashSessionRepository.GetActiveSessionAsync(cancellationToken)
            ?? throw new InvalidOperationException("Debe abrir la caja antes de registrar ventas.");

        var sale = new Sale
        {
            SaleNumber = saleNumber,
            CreatedAt = nowUtc,
            PaymentMethod = request.PaymentMethod,
            CashSessionId = activeSession.Id,
            UserId = userId
        };

        foreach (var itemRequest in request.Items)
        {
            if (itemRequest.Quantity <= 0)
                throw new InvalidOperationException(
                    $"La cantidad del producto Id {itemRequest.ProductId} debe ser mayor a cero.");

            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);

            if (product is null)
                throw new InvalidOperationException(
                    $"Producto con Id {itemRequest.ProductId} no encontrado.");

            if (!product.IsActive)
                throw new InvalidOperationException(
                    $"El producto '{product.Name}' no esta activo.");

            // AddItem valida stock y lanza si es insuficiente.
            // El decremento de stock queda trackeado por EF Core y se persiste
            // en el mismo SaveChangesAsync.
            sale.AddItem(product, itemRequest.Quantity);
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(sale);
    }

    public async Task<IReadOnlyList<SaleDto>> GetSalesByDateAsync(DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        var sales = await _saleRepository.GetByDateRangeAsync(dayStart, dayEnd, cancellationToken);

        return sales.Select(MapToDto).ToList();
    }

    public async Task<SaleDto?> GetSaleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetByIdWithItemsAsync(id, cancellationToken);
        return sale is null ? null : MapToDto(sale);
    }

    // ------------------------------------------------------------
    // Privados
    // ------------------------------------------------------------

    private async Task<string> GenerateSaleNumberAsync(DateTime dateUtc, CancellationToken cancellationToken)
    {
        var lastNumber = await _saleRepository.GetLastSaleNumberForDateAsync(dateUtc, cancellationToken);
        var nextConsecutive = 1;

        if (!string.IsNullOrEmpty(lastNumber))
        {
            // Formato esperado: V-YYYYMMDD-NNNN. Los ultimos 4 caracteres son el consecutivo.
            var lastConsecutiveStr = lastNumber.Substring(lastNumber.Length - 4);
            if (int.TryParse(lastConsecutiveStr, out var lastConsecutive))
                nextConsecutive = lastConsecutive + 1;
        }

        return $"V-{dateUtc:yyyyMMdd}-{nextConsecutive:D4}";
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            CreatedAt = sale.CreatedAt,
            Total = sale.Total,
            PaymentMethod = sale.PaymentMethod,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}