
using CareService.Application.Interfaces;
using CareService.Application.Interfaces.Services;
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
            services.AddScoped<IConsultationRepository,ConsultationRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<ILabOrderRepository, LabOrderRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();

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

            // User Service HTTP Client
            var userServiceBaseUrl =
                configuration["Services:UserService:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "UserService BaseUrl is missing.");

            services.AddHttpClient<
                ILabDirectoryClient,
                LabDirectoryClient>(
                client =>
                {
                    client.BaseAddress = new Uri(userServiceBaseUrl);
                })
                .AddHttpMessageHandler<AuthorizationHeaderHandler>();

            services.AddHttpClient<
                IUserProfileClient,
                UserProfileClient>(
                client =>
                {
                    client.BaseAddress = new Uri(userServiceBaseUrl);
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
