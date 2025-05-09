    using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using E_LaptopShop.Application.Common.Behaviors;

namespace E_LaptopShop.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Add AutoMapper
            services.AddAutoMapper(assembly);

            // Add MediatR with validation pipeline
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            // Add FluentValidation
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
