using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Services;
using AppointmentService.Infrastructure.Clients;
using AppointmentService.Infrastructure.Persistence;
using AppointmentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // PostgreSQL DbContext
            services.AddDbContext<AppointmentDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();

            services.AddScoped<IDoctorUnavailablePeriodRepository, DoctorUnavailablePeriodRepository>();

            // Application service
            services.AddScoped<IAppointmentService,AppointmentService.Application.Services.AppointmentService>();
           

            // Access the current incoming HTTP request
            services.AddHttpContextAccessor();

            // Register the authorization forwarding handler
            services.AddTransient<AuthorizationHeaderHandler>();

            // Validate UserService BaseUrl
            var userServiceBaseUrl =
                configuration["Services:UserService:BaseUrl"];

            if (string.IsNullOrWhiteSpace(userServiceBaseUrl) ||
                !Uri.TryCreate(
                    userServiceBaseUrl,
                    UriKind.Absolute,
                    out var baseUri) ||
                (baseUri.Scheme != Uri.UriSchemeHttp &&
                 baseUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException(
                    "Services:UserService:BaseUrl is missing or invalid.");
            }

            // Typed HttpClient for UserService
            services.AddHttpClient<
                IUserServiceClient,
                UserServiceClient>(client =>
                {
                    client.BaseAddress = new Uri(
                        baseUri.AbsoluteUri.TrimEnd('/') + "/");

                    client.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<AuthorizationHeaderHandler>();

            return services;
        }
    }
}