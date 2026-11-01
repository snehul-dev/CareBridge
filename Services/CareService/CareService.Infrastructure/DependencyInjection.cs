using CareService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection service,
            IConfiguration configuration
            )
        {
            service.AddDbContext<CareDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));


            return service;
        }
    }
}
