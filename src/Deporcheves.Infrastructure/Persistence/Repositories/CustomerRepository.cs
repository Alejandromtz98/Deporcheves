using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DeporchevesDbContext _context;
    public CustomerRepository(DeporchevesDbContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Customer>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term)) 
            return await _context.Customers.Where(c => c.IsActive).Take(50).ToListAsync(cancellationToken);
        
        var t = term.ToLower();
        return await _context.Customers
            .Where(c => c.IsActive && c.Name.ToLower().Contains(t))
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        => await _context.Customers.AddAsync(customer, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}