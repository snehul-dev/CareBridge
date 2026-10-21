using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Mappings;
using UserService.Application.Validators;

namespace UserService.Application
{
    public static  class DependencyInjection
    {
      public static IServiceCollection AddApplication(
          this IServiceCollection services
          )
        {
            services.AddValidatorsFromAssemblyContaining<RegisterPatientValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterDoctorValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterLabValidator>();

            services.AddAutoMapper(
             cfg => { },
             typeof(UserMappingProfile).Assembly);


            return services;
        }
    }
}
