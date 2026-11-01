using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using System.Text;

namespace CareService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services
            )
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
