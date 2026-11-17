using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using System.Text;
using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Services;

namespace AppointmentService.Application
{
    public static class DependencyInjection
    {
     public static IServiceCollection AddApplication(
            this IServiceCollection services
            )
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();


            return services;
        }
    }
}
