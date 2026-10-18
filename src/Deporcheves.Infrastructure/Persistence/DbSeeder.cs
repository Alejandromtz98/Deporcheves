using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(DeporchevesDbContext context)
    {
        // Verifica si ya existen usuarios
        if (await context.Users.AnyAsync())
            return;

        var admin = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = UserRole.Admin,
            IsActive = true
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}