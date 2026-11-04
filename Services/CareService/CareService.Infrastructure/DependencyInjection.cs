
using CareService.Application.Interfaces;
using CareService.Infrastructure.ExternalServices;
using CareService.Infrastructure.Persistence;
using CareService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "CareService database connection string is missing.");

            services.AddDbContext<CareDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Repositories
            services.AddScoped<
                IConsultationRepository,
                ConsultationRepository>();

            // HTTP Context Accessor
            services.AddHttpContextAccessor();

            // Authorization Header Handler
            services.AddTransient<AuthorizationHeaderHandler>();

            // Appointment Service HTTP Client
            var appointmentServiceBaseUrl =
                configuration["Services:AppointmentService:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "AppointmentService BaseUrl is missing.");

            services.AddHttpClient<
                IAppointmentServiceClient,
                AppointmentServiceClient>(
                client =>
                {
                    client.BaseAddress =
                        new Uri(appointmentServiceBaseUrl);
                })
                .AddHttpMessageHandler<AuthorizationHeaderHandler>();

            // Meeting Link Provider
            services.AddScoped<
                IMeetingLinkProvider,
                JitsiMeetingLinkProvider>();

            return services;
        }
    }
}
