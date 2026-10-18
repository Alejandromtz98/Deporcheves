using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Abstractions.Security;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;

    public UserService(IUserRepository repository, IPasswordHasher hasher)
    {
        _repository = repository;
        _hasher = hasher;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task CreateAsync(string username, string password, UserRole role, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Usuario y contrasena son obligatorios.");

        if (await _repository.ExistsWithUsernameAsync(username, cancellationToken))
            throw new InvalidOperationException("El nombre de usuario ya esta en uso.");

        var user = new User
        {
            Username = username.Trim(),
            PasswordHash = _hasher.Hash(password),
            Role = role,
            IsActive = true
        };

        await _repository.AddAsync(user, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}