
using Amazon;
using Amazon.S3;
using CareService.Application.Interfaces;
using CareService.Application.Interfaces.Services;
using CareService.Infrastructure.Configurations;
using CareService.Infrastructure.ExternalServices;
using CareService.Infrastructure.Messaging;
using CareService.Infrastructure.Pdf;
using CareService.Infrastructure.Persistence;
using CareService.Infrastructure.Repositories;
using CareService.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

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

            services.Configure<AwsSettings>(
                configuration.GetSection("AWS")
                );


            services.AddSingleton<IAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<AwsSettings>>().Value;

                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(
                        settings.Region)
                };

                if (!string.IsNullOrWhiteSpace(settings.AccessKey) &&
                    !string.IsNullOrWhiteSpace(settings.SecretKey))
                {
                    return new AmazonS3Client(
                        settings.AccessKey.Trim(),
                        settings.SecretKey.Trim(),
                        config);
                }

                return new AmazonS3Client(config);
            });

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = configuration.GetValue<int>("RabbitMQ:Port", 5672),
                    UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = configuration["RabbitMQ:Password"] ?? "guest",
                    AutomaticRecoveryEnabled = true
                };

                return factory.CreateConnectionAsync()
                    .GetAwaiter()
                    .GetResult();
            });

            services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
            services.AddHostedService<PrescriptionPdfConsumer>();

            services.AddScoped<IFileStorageService, S3FileStorageService>();
            services.AddScoped<IPrescriptionPdfGenerator, PrescriptionPdfGenerator>();

            // Repositories
            services.AddScoped<IConsultationRepository,ConsultationRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<ILabOrderRepository, LabOrderRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ILabResultRepository, LabResultRepository>();

            // HTTP Context Accessor
            services.AddHttpContextAccessor();

            // Authorization Header Handler
            services.AddTransient<AuthorizationHeaderHandler>();

            // Service-to-Service Token Handler
            services.AddTransient<ServiceTokenHandler>();

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
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // Internal lab directory (service-to-service)
            services.AddHttpClient<
                IInternalLabClient,
                InternalLabClient>(
                client =>
                {
                    client.BaseAddress = new Uri(userServiceBaseUrl);
                })
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // Appointment statistics (service-to-service)
            services.AddHttpClient<
                IAppointmentStatsClient,
                AppointmentStatsClient>(
                client =>
                {
                    client.BaseAddress =
                        new Uri(appointmentServiceBaseUrl);
                })
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // User statistics (service-to-service)
            services.AddHttpClient<
                IUserStatsClient,
                UserStatsClient>(
                client =>
                {
                    client.BaseAddress = new Uri(userServiceBaseUrl);
                })
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // Meeting Link Provider
            services.AddScoped<
                IMeetingLinkProvider,
                JitsiMeetingLinkProvider>();

            return services;
        }
    }
}
