using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

public interface IPayrollRepository
{
    Task<IReadOnlyList<Payroll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default);
    Task RegisterPayrollAsync(int employeeId, DateTime start, DateTime end, DateTime paymentDate, decimal baseSalary, decimal advances, decimal deductions, decimal adjustments, string paymentMethod, string? reference, CancellationToken cancellationToken = default);
}