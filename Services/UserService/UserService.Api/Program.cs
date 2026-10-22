
using CareBridge.Shared.Middlewares;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Net;
using System.Security.Claims;
using System.Text;
using UserService.Application;
using UserService.Infrastructure;
using UserService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register application and infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Register controllers
builder.Services.AddControllers();

// Standardize automatic model validation errors
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(entry => entry.Value!.Errors)
            .Select(error =>
                string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Invalid input."
                    : error.ErrorMessage)
            .ToList();

        var response = ApiResponse<object>.Fail(
            errors,
            "Validation failed",
            HttpStatusCode.BadRequest);

        return new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    };
});

builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWT Bearer authentication
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] =
                new List<string>()
        });
});

// JWT authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "JWT Secret is missing from configuration.");

        var issuer = builder.Configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is missing from configuration.");

        var audience = builder.Configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is missing from configuration.");

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)),

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                RoleClaimType = ClaimTypes.Role
            };

        // Standardized unauthorized and forbidden responses
        options.Events = new JwtBearerEvents
        {
            // Handle unauthorized requests (401)
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                var response = ApiResponse<object>.Fail(
                    new[]
                    {
                        "A valid access token is required."
                    },
                    "Unauthorized",
                    HttpStatusCode.Unauthorized);

                await context.Response.WriteAsJsonAsync(
                    response,
                    cancellationToken:
                        context.HttpContext.RequestAborted);
            },

            // Handle authenticated users without permission (403)
            OnForbidden = async context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                var response = ApiResponse<object>.Fail(
                    new[]
                    {
                        "You do not have permission to perform this action."
                    },
                    "Forbidden",
                    HttpStatusCode.Forbidden);

                await context.Response.WriteAsJsonAsync(
                    response,
                    cancellationToken:
                        context.HttpContext.RequestAborted);
            }
        };
    });

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed admin user
await AdminSeeder.SeedAsync(app.Services);

// Global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection
app.UseHttpsRedirection();

// Authentication must run before authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

app.Run();
