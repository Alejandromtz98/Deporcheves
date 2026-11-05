using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public class ReceivableService : IReceivableService
{
    private readonly IReceivableRepository _repository;

    public ReceivableService(IReceivableRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AccountsReceivable>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetPendingAsync(cancellationToken);
    }

    public async Task RegisterPaymentAsync(int id, decimal amount, CancellationToken cancellationToken = default)
    {
        var receivable = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Cuenta por cobrar no encontrada.");

        // Llama al método de dominio que valida que no pague de más y actualiza el saldo/estado
        receivable.RegisterPayment(amount); 
        
        await _repository.SaveChangesAsync(cancellationToken);
    }
}