using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public class ReceivableService : IReceivableService
{
    private readonly IReceivableRepository _repository;
    private readonly ICashSessionRepository _cashRepository; // NUEVO

    public ReceivableService(IReceivableRepository repository, ICashSessionRepository cashRepository)
    {
        _repository = repository;
        _cashRepository = cashRepository;
    }

    public async Task<IReadOnlyList<AccountsReceivable>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetPendingAsync(cancellationToken);
    }

    public async Task RegisterPaymentAsync(int id, decimal amount, CancellationToken cancellationToken = default)
    {
        var receivable = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Cuenta por cobrar no encontrada.");

        // 1. Validar que haya una caja abierta para guardar el dinero físico
        var activeSession = await _cashRepository.GetActiveSessionAsync(cancellationToken)
            ?? throw new InvalidOperationException("Debe abrir la caja antes de registrar cobros de fiado.");

        // 2. Modificar la deuda
        receivable.RegisterPayment(amount); 
        
        // 3. Registrar el ingreso en la caja
        activeSession.RegisterMovement(
            CashMovementType.Income, 
            amount, 
            $"Cobro de fiado - Cliente: {receivable.Customer.Name}");

        await _repository.SaveChangesAsync(cancellationToken);
    }
}