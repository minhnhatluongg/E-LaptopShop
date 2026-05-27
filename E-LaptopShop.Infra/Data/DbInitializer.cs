using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Infra.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Đảm bảo DB đã được migrate
                await context.Database.MigrateAsync();

                // Chỉ seed nếu chưa có user nào
                if (await context.Users.AnyAsync())
                {
                    logger.LogInformation("Database already has users. Skipping seed.");
                    return;
                }

                // Lấy RoleId từ DB (đã seed bởi HasData)
                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == RoleCodes.Admin);
                var customerRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == RoleCodes.Customer);

                if (adminRole == null || customerRole == null)
                {
                    logger.LogWarning("Roles not found. Make sure migrations have been applied.");
                    return;
                }

                var seedPassword = "Test@123";

                var users = new List<User>
                {
                    new User
                    {
                        FirstName = "Admin",
                        LastName = "System",
                        Email = "admin@elaptopshop.com",
                        PasswordHash = hasher.HashPassword(seedPassword),
                        Phone = "0901234567",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        RoleId = adminRole.Id,
                        IsActive = true,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new User
                    {
                        FirstName = "Nguyen",
                        LastName = "Customer",
                        Email = "customer@elaptopshop.com",
                        PasswordHash = hasher.HashPassword(seedPassword),
                        Phone = "0912345678",
                        Gender = "Female",
                        DateOfBirth = new DateTime(1995, 6, 15),
                        RoleId = customerRole.Id,
                        IsActive = true,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} users successfully.", users.Count);
                logger.LogInformation("=== SEED ACCOUNTS ===");
                logger.LogInformation("Admin  : admin@elaptopshop.com / {Password}", seedPassword);
                logger.LogInformation("Customer: customer@elaptopshop.com / {Password}", seedPassword);
                logger.LogInformation("=====================");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding database");
            }
        }
    }
}
