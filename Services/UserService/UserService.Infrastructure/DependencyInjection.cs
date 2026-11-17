using Amazon;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Infrastructure.Configurations;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Security;
using UserService.Infrastructure.Storage;

namespace UserService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
           this IServiceCollection service,
            IConfiguration configuration
            )
        {

            //database
            service.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );

            // AWS S3 storage
            service.Configure<AwsSettings>(configuration.GetSection("AWS"));
            service.AddSingleton<IAmazonS3>(sp =>
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

            service.AddScoped<IFileStorageService, S3FileStorageService>();

            service.AddScoped<IPasswordHasherService, PasswordHasherService>();

            //repositories
            service.AddScoped<IRoleRepository, RoleRepository>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IDoctorRepository, DoctorRepository>();
            service.AddScoped<IDoctorService, DoctorService>();
            service.AddScoped<ILabRepository, LabRepository>();
            service.AddScoped<IAdminRepository, AdminRepository>();


            //services
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            service.AddScoped<IUserProfileService, UserProfileService>();
            service.AddScoped<IProfilePictureService, ProfilePictureService>();

            return service;
        }
    }
}
