using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using myfinance.API.Errors;
using myfinance.Application.Services;
using myfinance.Application.Services.Interfaces;
using myfinance.Domain.Entities;
using myfinance.Infrastructure.Context;
using myfinance.Infrastructure.Repositories;
using myfinance.Infrastructure.Repositories.Interfaces;
using myfinance.Infrastructure.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MyFinanceContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyFinanceDatabase"));
});

builder.Services.AddControllers();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("u4aQm7M3r0K7mY3rK8v4Yk2kH5q8mD0iT4oY7Qx2N9eV1uL6cP3aR5xF8bW2jH9sQ0zT6nM4cJ1pK7wL2dA=="))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
    ;
});

app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var errors = exception switch
        {
            UnauthorizedAccessException ex => new Error
            (
                "UANUTHORIZED_ACCESS",
                ex.Message,
                (int)HttpStatusCode.Unauthorized
            ),
            _ => new Error
            (
                "INTERNAL_SERVER_ERROR",
                "An unexpected error occured",
                (int)HttpStatusCode.InternalServerError
            )
        };

        context.Response.StatusCode = errors.Status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(errors);
    });
});

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
