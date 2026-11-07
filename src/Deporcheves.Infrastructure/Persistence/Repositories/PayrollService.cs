using Deporcheves.Domain.Entities;
using Deporcheves.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly DeporchevesDbContext _context;

    public PayrollRepository(DeporchevesDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Payroll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .Include(p => p.Employee)
            .OrderByDescending(p => p.PaymentDate)
            .Take(50)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees.Where(e => e.IsActive).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task RegisterPayrollAsync(int employeeId, DateTime start, DateTime end, DateTime paymentDate, decimal baseSalary, decimal advances, decimal deductions, decimal adjustments, string paymentMethod, string? reference, CancellationToken cancellationToken = default)
    {
        var totalCalculated = baseSalary - advances - deductions + adjustments;
        if (totalCalculated < 0) throw new InvalidOperationException("El importe calculado de nómina no puede ser negativo.");

        var payroll = new Payroll
        {
            EmployeeId = employeeId,
            PeriodStart = DateTime.SpecifyKind(start, DateTimeKind.Utc),
            PeriodEnd = DateTime.SpecifyKind(end, DateTimeKind.Utc),
            PaymentDate = DateTime.SpecifyKind(paymentDate, DateTimeKind.Utc),
            BaseSalary = baseSalary,
            Advances = advances,
            Deductions = deductions,
            Adjustments = adjustments,
            TotalCalculated = totalCalculated,
            AmountPaid = totalCalculated,
            Balance = 0,
            PaymentMethod = paymentMethod,
            Status = "PAGADA", // La entidad espera un string
            Reference = reference
        };

        _context.Payrolls.Add(payroll);
        await _context.SaveChangesAsync(cancellationToken);
    }
}