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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
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

            // AWS Settings
            services.Configure<AwsSettings>(
                configuration.GetSection("AWS"));

            // AWS S3 Authentication
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<
                    IOptions<AwsSettings>>().Value;

                var environment =
                    sp.GetRequiredService<IHostEnvironment>();

                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(
                        settings.Region)
                };

                // Local Development - Access Key and Secret Key
                if (environment.IsDevelopment())
                {
                    if (!string.IsNullOrWhiteSpace(settings.AccessKey) &&
                        !string.IsNullOrWhiteSpace(settings.SecretKey))
                    {
                        return new AmazonS3Client(
                            settings.AccessKey.Trim(),
                            settings.SecretKey.Trim(),
                            config);
                    }
                }

                // Production - Default Credential Chain (EC2 IAM Role)
                return new AmazonS3Client(config);
            });

            // RabbitMQ Connection
            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:HostName"]
                        ?? "localhost",

                    Port = configuration.GetValue<int>(
                        "RabbitMQ:Port", 5672),

                    UserName = configuration["RabbitMQ:UserName"]
                        ?? "guest",

                    Password = configuration["RabbitMQ:Password"]
                        ?? "guest",

                    AutomaticRecoveryEnabled = true
                };

                return factory.CreateConnectionAsync()
                    .GetAwaiter()
                    .GetResult();
            });

            // RabbitMQ Publisher and Consumer
            services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
            services.AddHostedService<PrescriptionPdfConsumer>();

            // File Storage and PDF Generation
            services.AddScoped<IFileStorageService, S3FileStorageService>();
            services.AddScoped<IPrescriptionPdfGenerator, PrescriptionPdfGenerator>();

            // Repositories
            services.AddScoped<IConsultationRepository, ConsultationRepository>();
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

            // User Profile Client
            services.AddHttpClient<
                IUserProfileClient,
                UserProfileClient>(
                client =>
                {
                    client.BaseAddress = new Uri(userServiceBaseUrl);
                })
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // Internal Lab Directory (Service-to-Service)
            services.AddHttpClient<
                IInternalLabClient,
                InternalLabClient>(
                client =>
                {
                    client.BaseAddress = new Uri(userServiceBaseUrl);
                })
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // Appointment Statistics (Service-to-Service)
            services.AddHttpClient<
                IAppointmentStatsClient,
                AppointmentStatsClient>(
                client =>
                {
                    client.BaseAddress =
                        new Uri(appointmentServiceBaseUrl);
                })
                .AddHttpMessageHandler<ServiceTokenHandler>();

            // User Statistics (Service-to-Service)
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