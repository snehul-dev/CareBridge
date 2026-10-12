
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var services = scope.ServiceProvider;

            var context =
                services.GetRequiredService<UserDbContext>();

            var configuration =
                services.GetRequiredService<IConfiguration>();

            var passwordHasher =
                services.GetRequiredService<IPasswordHasherService>();

            // Read admin credentials from User Secrets
            var email = configuration["AdminSeed:Email"]?
                .Trim()
                .ToLowerInvariant();

            var password = configuration["AdminSeed:Password"];

            var phoneNumber =
                configuration["AdminSeed:PhoneNumber"]?.Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new InvalidOperationException(
                    "Admin seed credentials are missing. " +
                    "Configure AdminSeed:Email, AdminSeed:Password, " +
                    "and AdminSeed:PhoneNumber.");
            }

            // Step 1: Find or create Admin role
            var adminRole = await context.Roles
                .SingleOrDefaultAsync(role => role.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin"
                };

                context.Roles.Add(adminRole);

                await context.SaveChangesAsync();
            }

            // Step 2: Find or create Admin user
            var adminUser = await context.Users
                .SingleOrDefaultAsync(user => user.Email == email);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    PhoneNumber = phoneNumber,
                    PasswordHash =
                        passwordHasher.HashPassword(password),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);

                await context.SaveChangesAsync();
            }

            // Step 3: Assign Admin role to the user
            var hasAdminRole = await context.UserRoles
                .AnyAsync(userRole =>
                    userRole.UserId == adminUser.Id &&
                    userRole.RoleId == adminRole.Id);

            if (!hasAdminRole)
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });

                await context.SaveChangesAsync();
            }
        }
    }
}