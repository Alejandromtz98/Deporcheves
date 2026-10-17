using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface ISaleRepository
{
    /// <summary>
    /// Obtiene una venta por Id, incluyendo sus items. Devuelve null si no existe.
    /// Solo lectura, sin tracking.
    /// </summary>
    Task<Sale?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve todas las ventas en el rango de fechas indicado, con sus items.
    /// Solo lectura, sin tracking. Ordenadas por fecha descendente.
    /// </summary>
    Task<IReadOnlyList<Sale>> GetByDateRangeAsync(
        DateTime fromUtc, DateTime toUtc,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve el ultimo SaleNumber registrado para una fecha especifica.
    /// Se usa para calcular el consecutivo del dia. Devuelve null si no hay ventas ese dia.
    /// </summary>
    Task<string?> GetLastSaleNumberForDateAsync(
        DateTime dateUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva venta al contexto.
    /// </summary>
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste todos los cambios pendientes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sale>> GetByCashSessionIdAsync(int cashSessionId, CancellationToken cancellationToken = default);
}