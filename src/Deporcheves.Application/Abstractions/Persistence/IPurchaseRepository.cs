using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface IPurchaseRepository
{
    /// <summary>
    /// Detalle completo: incluye Supplier e Items.
    /// </summary>
    Task<Purchase?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Listado para reportes: incluye Supplier, sin Items. Rango semiabierto [from, to).
    /// </summary>
    Task<IReadOnlyList<Purchase>> GetByDateRangeAsync(
        DateTime fromUtc, DateTime toUtc,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ultimo PurchaseNumber de un dia para calcular el consecutivo.
    /// </summary>
    Task<string?> GetLastPurchaseNumberForDateAsync(
        DateTime dateUtc, CancellationToken cancellationToken = default);

    Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}