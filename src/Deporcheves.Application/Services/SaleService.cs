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
    private readonly ICustomerRepository _customerRepository;       // NUEVO
    private readonly IReceivableRepository _receivableRepository;   // NUEVO

    public SaleService(
        ISaleRepository saleRepository, 
        IProductRepository productRepository, 
        ICashSessionRepository cashSessionRepository,
        IInventoryService inventoryService,
        ICustomerRepository customerRepository,      // NUEVO
        IReceivableRepository receivableRepository)  // NUEVO
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

        // Validar Cliente si hay fiado
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
            PaymentMethod = request.PaymentMethod,
            CashSessionId = activeSession.Id,
            UserId = userId,
            CustomerId = customer?.Id,          // Asignar cliente
            AmountFiado = request.AmountFiado   // Asignar monto fiado
        };

        foreach (var itemRequest in request.Items)
        {
            if (itemRequest.Quantity <= 0)
                throw new InvalidOperationException($"La cantidad del producto Id {itemRequest.ProductId} debe ser mayor a cero.");

            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken)
                ?? throw new InvalidOperationException($"Producto con Id {itemRequest.ProductId} no encontrado.");

            if (!product.IsActive)
                throw new InvalidOperationException($"El producto '{product.Name}' no esta activo.");

            sale.AddItem(product, itemRequest.Quantity);
            
            // Registrar salida en el Kardex (InventoryService)
            await _inventoryService.RegisterMovementAsync(product, InventoryMovementType.Sale, -itemRequest.Quantity, product.Cost, saleNumber, cancellationToken: cancellationToken);
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        
        // Si hay fiado, generamos la deuda antes de guardar para que sea una sola transacción
        if (request.AmountFiado > 0 && customer != null)
        {
            var receivable = new AccountsReceivable(sale, customer.Id, request.AmountFiado);
            await _receivableRepository.AddAsync(receivable, cancellationToken);
        }

        // Guarda la Venta, los Movimientos de Inventario y la Deuda en un solo golpe.
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