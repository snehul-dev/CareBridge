using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Read JWT configuration (same keys as the downstream services)
var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["Secret"] ?? jwtSettings["SecretKey"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException(
        "Jwt:Secret is not configured. Set Jwt__Secret (or Jwt__SecretKey) before starting the API Gateway.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy")
    );

var app = builder.Build();

app.UseCors("FrontendPolicy");

// Internal service-to-service endpoints are called directly between
// containers and must never be reachable through the public gateway.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;

    if (path.Contains("/internal/", StringComparison.OrdinalIgnoreCase) ||
        path.EndsWith("/internal", StringComparison.OrdinalIgnoreCase))
    {
        await WriteGatewayErrorAsync(
            context,
            StatusCodes.Status404NotFound,
            "The requested API endpoint was not found.");

        return;
    }

    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

// Unmatched routes return a JSON body so clients can surface a message
// instead of an empty response.
app.MapFallback(async context =>
{
    await WriteGatewayErrorAsync(
        context,
        StatusCodes.Status404NotFound,
        "The requested API endpoint was not found.");
});

app.Run();

static async Task WriteGatewayErrorAsync(
    HttpContext context,
    int statusCode,
    string message)
{
    if (context.Response.HasStarted)
    {
        return;
    }

    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";

    await context.Response.WriteAsJsonAsync(new
    {
        isSuccess = false,
        statusCode,
        data = (object?)null,
        message,
        errors = new[] { message }
    });
}
