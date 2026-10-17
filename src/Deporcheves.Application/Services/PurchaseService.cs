using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Purchases;
using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public PurchaseService(
        IPurchaseRepository purchaseRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _purchaseRepository = purchaseRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<PurchaseDto> RegisterPurchaseAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("La compra debe contener al menos un item.");

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken)
            ?? throw new InvalidOperationException($"Proveedor con Id {request.SupplierId} no encontrado.");

        if (!supplier.IsActive)
            throw new InvalidOperationException($"El proveedor '{supplier.Name}' no esta activo.");

        var nowUtc = DateTime.UtcNow;
        var purchaseNumber = await GeneratePurchaseNumberAsync(nowUtc, cancellationToken);

        var purchase = new Purchase
        {
            PurchaseNumber = purchaseNumber,
            SupplierId = supplier.Id,
            CreatedAt = nowUtc,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        // Agrupamos por ProductId para evitar trackear dos instancias del mismo producto
        // si el usuario agrego el mismo SKU dos veces en el formulario.
        var groupedItems = request.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new CreatePurchaseItemRequest
            {
                ProductId = g.Key,
                Quantity = g.Sum(i => i.Quantity),
                UnitCost = g.First().UnitCost
            });

        foreach (var itemRequest in groupedItems)
        {
            if (itemRequest.Quantity <= 0)
                throw new InvalidOperationException(
                    $"La cantidad del producto Id {itemRequest.ProductId} debe ser mayor a cero.");

            if (itemRequest.UnitCost < 0)
                throw new InvalidOperationException(
                    $"El costo del producto Id {itemRequest.ProductId} no puede ser negativo.");

            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Producto con Id {itemRequest.ProductId} no encontrado.");

            purchase.AddItem(product, itemRequest.Quantity, itemRequest.UnitCost);
        }

        await _purchaseRepository.AddAsync(purchase, cancellationToken);
        await _purchaseRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(purchase, supplier.Name);
    }

    public async Task<IReadOnlyList<PurchaseDto>> GetPurchasesByDateAsync(DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        var purchases = await _purchaseRepository.GetByDateRangeAsync(dayStart, dayEnd, cancellationToken);

        // Nota: GetByDateRangeAsync no carga Items. El detalle con items se obtiene
        // por GetPurchaseByIdAsync. Aqui devolvemos el resumen sin items.
        return purchases.Select(p => new PurchaseDto
        {
            Id = p.Id,
            PurchaseNumber = p.PurchaseNumber,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.Name ?? string.Empty,
            CreatedAt = p.CreatedAt,
            Total = p.Total,
            Notes = p.Notes,
            Items = new List<PurchaseItemDto>()
        }).ToList();
    }

    public async Task<PurchaseDto?> GetPurchaseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetByIdWithItemsAsync(id, cancellationToken);
        if (purchase is null) return null;
        return MapToDto(purchase, purchase.Supplier?.Name ?? string.Empty);
    }

    private async Task<string> GeneratePurchaseNumberAsync(DateTime dateUtc, CancellationToken cancellationToken)
    {
        var lastNumber = await _purchaseRepository.GetLastPurchaseNumberForDateAsync(dateUtc, cancellationToken);
        var nextConsecutive = 1;

        if (!string.IsNullOrEmpty(lastNumber))
        {
            var lastConsecutiveStr = lastNumber.Substring(lastNumber.Length - 4);
            if (int.TryParse(lastConsecutiveStr, out var lastConsecutive))
                nextConsecutive = lastConsecutive + 1;
        }

        return $"C-{dateUtc:yyyyMMdd}-{nextConsecutive:D4}";
    }

    private static PurchaseDto MapToDto(Purchase purchase, string supplierName)
    {
        return new PurchaseDto
        {
            Id = purchase.Id,
            PurchaseNumber = purchase.PurchaseNumber,
            SupplierId = purchase.SupplierId,
            SupplierName = supplierName,
            CreatedAt = purchase.CreatedAt,
            Total = purchase.Total,
            Notes = purchase.Notes,
            Items = purchase.Items.Select(i => new PurchaseItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}