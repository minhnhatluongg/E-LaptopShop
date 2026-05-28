using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Enums;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

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
                // Nếu DB đã có sẵn table nhưng chưa có migration history → tự baseline
                try
                {
                    await context.Database.MigrateAsync();
                }
                catch (SqlException ex) when (ex.Number == 2714) // Object already exists
                {
                    logger.LogWarning("Tables already exist without migration history. Registering migration as applied...");
                    await EnsureMigrationHistoryAsync(context, logger);
                }

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

        private static async Task EnsureMigrationHistoryAsync(ApplicationDbContext context, ILogger logger)
        {
            var sql = """
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory'
                )
                BEGIN
                    CREATE TABLE [__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                    );
                END

                IF NOT EXISTS (
                    SELECT 1 FROM [__EFMigrationsHistory]
                    WHERE [MigrationId] = '20260528020452_InitialCreate'
                )
                BEGIN
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                    VALUES ('20260528020452_InitialCreate', '9.0.4');
                END
                """;

            await context.Database.ExecuteSqlRawAsync(sql);
            logger.LogInformation("Migration history baseline completed.");
        }
    }
}
