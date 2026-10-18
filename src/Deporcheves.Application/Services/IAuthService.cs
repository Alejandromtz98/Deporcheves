using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public interface IAuthService
{
    Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
}