using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using System.Text;
using CareService.Application.Mappings;
using CareService.Application.Interfaces;
using CareService.Application.Services;

namespace CareService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services
            )
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IConsultationService, ConsultationService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();

            services.AddAutoMapper(
            cfg => { },
             typeof(CareMappingProfile).Assembly);
            return services;

            
        }
    }
}
