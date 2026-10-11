using UserService.Api.Middleware;
using UserService.Application;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//register services

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.Run();