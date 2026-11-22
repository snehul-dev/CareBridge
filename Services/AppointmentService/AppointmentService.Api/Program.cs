
using CareBridge.Shared.Middlewares;
using CareBridge.Shared.Responses;
using System.Net;
using System.Security.Claims;
using System.Text;
using AppointmentService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using AppointmentService.Application.Mappings;
using AppointmentService.Application;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger JWT configuration
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

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// UserService HttpClient
builder.Services.AddHttpClient("UserService", client =>
{
    var baseUrl = builder.Configuration[
        "Services:UserService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException(
            "UserService base URL is missing from configuration.");
    }

    client.BaseAddress = new Uri(baseUrl);
});

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

    // JWT diagnostic events
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine(
                $"JWT Authentication Failed: {context.Exception.Message}");

            return Task.CompletedTask;
        },

        // Standardized unauthorized response (401)
        OnChallenge = async context =>
        {
            Console.WriteLine(
                $"JWT Challenge: {context.Error} - {context.ErrorDescription}");

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

        // Standardized forbidden response (403)
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CareServiceOnly", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("client_id", "CareService");
    });
});

// AutoMapper
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(AppointmentMapping).Assembly);

var app = builder.Build();

// Global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication must run before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
