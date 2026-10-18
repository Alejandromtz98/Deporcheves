using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Abstractions.Security;
using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        
        if (user is null || !user.IsActive)
            return null;

        if (!_passwordHasher.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}