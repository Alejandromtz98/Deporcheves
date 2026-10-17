using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Suppliers;
using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _repository;

    public SupplierService(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SupplierDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _repository.GetAllActiveAsync(cancellationToken);
        return suppliers.Select(MapToDto).ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(id, cancellationToken);
        return supplier is null ? null : MapToDto(supplier);
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("El nombre del proveedor es obligatorio.");

        var name = request.Name.Trim();

        if (await _repository.ExistsWithNameAsync(name, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Ya existe un proveedor con el nombre '{name}'.");

        var supplier = new Supplier
        {
            Name = name,
            ContactName = Clean(request.ContactName),
            Phone = Clean(request.Phone),
            Email = Clean(request.Email),
            Address = Clean(request.Address)
        };

        await _repository.AddAsync(supplier, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(supplier);
    }

    public async Task<SupplierDto> UpdateAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("El nombre del proveedor es obligatorio.");

        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Proveedor con Id {request.Id} no encontrado.");

        var name = request.Name.Trim();

        if (await _repository.ExistsWithNameAsync(name, request.Id, cancellationToken))
            throw new InvalidOperationException($"Ya existe otro proveedor con el nombre '{name}'.");

        supplier.Name = name;
        supplier.ContactName = Clean(request.ContactName);
        supplier.Phone = Clean(request.Phone);
        supplier.Email = Clean(request.Email);
        supplier.Address = Clean(request.Address);
        supplier.UpdatedAt = DateTime.UtcNow;

        _repository.Update(supplier);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(supplier);
    }

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SupplierDto MapToDto(Supplier s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        ContactName = s.ContactName,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        IsActive = s.IsActive
    };
}