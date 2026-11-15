using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Sales;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly IInventoryService _inventoryService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IReceivableRepository _receivableRepository;

    public SaleService(
        ISaleRepository saleRepository, 
        IProductRepository productRepository, 
        ICashSessionRepository cashSessionRepository,
        IInventoryService inventoryService,
        ICustomerRepository customerRepository,
        IReceivableRepository receivableRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _cashSessionRepository = cashSessionRepository;
        _inventoryService = inventoryService;
        _customerRepository = customerRepository;
        _receivableRepository = receivableRepository;
    }

    public async Task<SaleDto> RegisterSaleAsync(CreateSaleRequest request, int userId, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La venta debe contener al menos un item.");

        var activeSession = await _cashSessionRepository.GetActiveSessionAsync(cancellationToken)
            ?? throw new InvalidOperationException("Debe abrir la caja antes de registrar ventas.");

        Customer? customer = null;
        if (request.AmountFiado > 0)
        {
            if (!request.CustomerId.HasValue)
                throw new InvalidOperationException("Debe seleccionar un cliente para registrar un fiado.");
            
            customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Cliente no encontrado.");
        }

        var nowUtc = DateTime.UtcNow;
        var saleNumber = await GenerateSaleNumberAsync(nowUtc, cancellationToken);

        var sale = new Sale
        {
            SaleNumber = saleNumber,
            CreatedAt = nowUtc,
            CashSessionId = activeSession.Id,
            UserId = userId,
            CustomerId = customer?.Id,
            
            // Asignar Pagos
            AmountCash = request.AmountCash,
            AmountTransfer = request.AmountTransfer,
            AmountCard = request.AmountCard,
            AmountFiado = request.AmountFiado
        };

        foreach (var itemRequest in request.Items)
        {
            if (itemRequest.Quantity <= 0)
                throw new InvalidOperationException($"La cantidad del producto Id {itemRequest.ProductId} debe ser mayor a cero.");

            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken)
                ?? throw new InvalidOperationException($"Producto con Id {itemRequest.ProductId} no encontrado.");

            if (!product.IsActive)
                throw new InvalidOperationException($"El producto '{product.Name}' no esta activo.");

            // Agregamos con el posible descuento
            sale.AddItem(product, itemRequest.Quantity, itemRequest.Discount);
            
            await _inventoryService.RegisterMovementAsync(product, InventoryMovementType.Sale, -itemRequest.Quantity, product.Cost, saleNumber, cancellationToken: cancellationToken);
        }

        // VALIDAR QUE LOS PAGOS CUADREN
        decimal totalPaid = sale.AmountCash + sale.AmountTransfer + sale.AmountCard + sale.AmountFiado;
        if (Math.Abs(totalPaid - sale.Total) > 0.01m)
        {
            throw new InvalidOperationException($"La suma de los pagos ({totalPaid:C2}) no coincide con el total de la venta ({sale.Total:C2}).");
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        
        if (sale.AmountFiado > 0 && customer != null)
        {
            var receivable = new AccountsReceivable(sale, customer.Id, sale.AmountFiado);
            await _receivableRepository.AddAsync(receivable, cancellationToken);
        }

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

    private async Task<string> GenerateSaleNumberAsync(DateTime dateUtc, CancellationToken cancellationToken)
    {
        var lastNumber = await _saleRepository.GetLastSaleNumberForDateAsync(dateUtc, cancellationToken);
        var nextConsecutive = 1;

        if (!string.IsNullOrEmpty(lastNumber))
        {
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
            
            // NUEVO: Mapeo de los pagos mixtos
            AmountCash = sale.AmountCash,
            AmountTransfer = sale.AmountTransfer,
            AmountCard = sale.AmountCard,
            AmountFiado = sale.AmountFiado,
            
            Items = sale.Items.Select(i => new SaleItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                Subtotal = i.Subtotal,
            }).ToList()
        };
    }
}