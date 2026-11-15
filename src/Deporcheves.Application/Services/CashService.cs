using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Cash;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public class CashService : ICashService
{
    private readonly ICashSessionRepository _cashRepository;
    private readonly ISaleRepository _saleRepository;

    public CashService(ICashSessionRepository cashRepository, ISaleRepository saleRepository)
    {
        _cashRepository = cashRepository;
        _saleRepository = saleRepository;
    }

    public async Task<CashReportDto> OpenSessionAsync(decimal openingCash, int userId, CancellationToken cancellationToken = default)
    {
        var active = await _cashRepository.GetActiveSessionAsync(cancellationToken);
        if (active != null)
            throw new InvalidOperationException("Ya existe un turno de caja abierto.");

        if (openingCash < 0)
            throw new InvalidOperationException("El fondo inicial no puede ser negativo.");

        var session = new CashSession { OpeningCash = openingCash, UserId = userId };
        await _cashRepository.AddAsync(session, cancellationToken);
        await _cashRepository.SaveChangesAsync(cancellationToken);

        return await BuildReportAsync(session, cancellationToken);
    }

    public async Task<CashReportDto> GetActiveSessionReportAsync(CancellationToken cancellationToken = default)
    {
        var session = await _cashRepository.GetActiveSessionAsync(cancellationToken)
            ?? throw new InvalidOperationException("No hay un turno de caja abierto.");

        return await BuildReportAsync(session, cancellationToken);
    }

    public async Task RegisterMovementAsync(CashMovementRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _cashRepository.GetActiveSessionAsync(cancellationToken)
            ?? throw new InvalidOperationException("No hay un turno de caja abierto.");

        session.RegisterMovement(request.Type, request.Amount, request.Reason);
        await _cashRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<CashReportDto> CloseSessionAsync(decimal closingCash, CancellationToken cancellationToken = default)
    {
        var session = await _cashRepository.GetActiveSessionAsync(cancellationToken)
            ?? throw new InvalidOperationException("No hay un turno de caja abierto.");

        var report = await BuildReportAsync(session, cancellationToken);
        session.Close(report.ExpectedCash, closingCash);
        
        await _cashRepository.SaveChangesAsync(cancellationToken);
        return await BuildReportAsync(session, cancellationToken);
    }

        private async Task<CashReportDto> BuildReportAsync(CashSession session, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetByCashSessionIdAsync(session.Id, cancellationToken);

        // Con el nuevo modelo de pagos mixtos, sumamos los montos individuales
        var cashSales = sales.Sum(s => s.AmountCash);
        var cardSales = sales.Sum(s => s.AmountCard);
        var transferSales = sales.Sum(s => s.AmountTransfer);

        var totalExpenses = session.Movements
            .Where(m => m.Type == CashMovementType.Expense).Sum(m => m.Amount);
        var totalWithdrawals = session.Movements
            .Where(m => m.Type == CashMovementType.Withdrawal).Sum(m => m.Amount);
        var totalIncomes = session.Movements
            .Where(m => m.Type == CashMovementType.Income).Sum(m => m.Amount);

        // Efectivo esperado = Fondo + Ventas Efectivo + Cobros CxC - Gastos - Retiros
        var expectedCash = session.OpeningCash + cashSales + totalIncomes - totalExpenses - totalWithdrawals;

        return new CashReportDto
        {
            SessionId = session.Id,
            OpenedAt = session.OpenedAt,
            ClosedAt = session.ClosedAt,
            OpeningCash = session.OpeningCash,
            CashSales = cashSales,
            CardSales = cardSales,
            TransferSales = transferSales,
            TotalExpenses = totalExpenses,
            TotalWithdrawals = totalWithdrawals,
            TotalIncomes = totalIncomes,
            ExpectedCash = expectedCash,
            ClosingCash = session.ClosingCash,
            Difference = session.Difference
        };
    }
}