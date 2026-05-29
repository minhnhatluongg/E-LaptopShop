using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Infra.Common;
using E_LaptopShop.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Infra
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProductSpecificationRepository, ProductSpecificationRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAuthRepository, UserAuthRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISysFileRepository, SysFileRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IInventoryHistoryRepository, InventoryHistoryRepository>();
            services.AddScoped<IUserAddressRepository,UserAddressRepository>();
            services.AddScoped<IRoleLookup, RoleLookupRepository>();
            services.AddScoped<ISlugGenerator, SlugGenerator>();
            services.AddScoped<IBrandRepositoy, BrandRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<IUserWalletRepository, UserWalletRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductCommentRepository, ProductCommentRepository>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            services.AddScoped<E_LaptopShop.Application.Services.Interfaces.ILoyaltyService,
                                E_LaptopShop.Infra.Services.LoyaltyService>();

            return services;
        }
    }
}
