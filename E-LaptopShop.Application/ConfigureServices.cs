using E_LaptopShop.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace E_LaptopShop.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Add AutoMapper
            services.AddAutoMapper(assembly);

            // Add MediatR with behaviors
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            // Add FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // ✨ Add Pipeline Behaviors (thứ tự quan trọng!)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
