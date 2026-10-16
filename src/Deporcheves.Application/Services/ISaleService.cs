using Deporcheves.Application.Dtos.Sales;

namespace Deporcheves.Application.Services;

public interface ISaleService
{
    /// <summary>
    /// Registra una venta. Valida productos, decrementa stock y persiste.
    /// Devuelve el DTO de la venta registrada.
    /// </summary>
    Task<SaleDto> RegisterSaleAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve las ventas de un dia especifico (en UTC).
    /// </summary>
    Task<IReadOnlyList<SaleDto>> GetSalesByDateAsync(DateTime dateUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve el detalle de una venta por Id. Null si no existe.
    /// </summary>
    Task<SaleDto?> GetSaleByIdAsync(int id, CancellationToken cancellationToken = default);
}