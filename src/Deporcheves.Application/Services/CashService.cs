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

    public async Task<CashReportDto> OpenSessionAsync(decimal openingCash, CancellationToken cancellationToken = default)
    {
        var active = await _cashRepository.GetActiveSessionAsync(cancellationToken);
        if (active != null)
            throw new InvalidOperationException("Ya existe un turno de caja abierto.");

        if (openingCash < 0)
            throw new InvalidOperationException("El fondo inicial no puede ser negativo.");

        var session = new CashSession { OpeningCash = openingCash };
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

        var cashSales = sales.Where(s => s.PaymentMethod == PaymentMethod.Cash).Sum(s => s.Total);
        var cardSales = sales.Where(s => s.PaymentMethod == PaymentMethod.Card).Sum(s => s.Total);
        var transferSales = sales.Where(s => s.PaymentMethod == PaymentMethod.Transfer).Sum(s => s.Total);

        var totalExpenses = session.Movements
            .Where(m => m.Type == CashMovementType.Expense).Sum(m => m.Amount);
        var totalWithdrawals = session.Movements
            .Where(m => m.Type == CashMovementType.Withdrawal).Sum(m => m.Amount);

        var expectedCash = session.OpeningCash + cashSales - totalExpenses - totalWithdrawals;

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
            ExpectedCash = expectedCash,
            ClosingCash = session.ClosingCash,
            Difference = session.Difference
        };
    }
}