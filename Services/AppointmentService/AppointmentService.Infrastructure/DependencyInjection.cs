using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Services;
using AppointmentService.Infrastructure.Persistence;
using AppointmentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
           this IServiceCollection services,
           IConfiguration configuration
            )
        {
            services.AddDbContext<AppointmentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();
            services.AddScoped<IDoctorUnavailablePeriodRepository, DoctorUnavailablePeriodRepository>();
            services.AddScoped<IAppointmentService, AppointmentService.Application.Services.AppointmentService>();
            return services;

        }
    }
}
