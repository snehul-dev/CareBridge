
using Microsoft.OpenApi;
using CareBridge.Shared.Middlewares;
using CareBridge.Shared.Responses;
using System.Net;
using CareService.Infrastructure;
using CareService.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register Controllers
builder.Services.AddControllers();


// JWT Authentication
builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorization();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CareBridge API",
        Version = "v1",
        Description = "CareBridge Backend API Documentation"
    });

    // JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token here."
    });

    // Apply JWT authentication requirement
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Dependency Injection
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
