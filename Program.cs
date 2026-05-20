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
using myfinance.Infrastructure.Config;
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

ConfigurationManager config = builder.Configuration;

builder.Services.Configure<MyFinanceSettings>(config.GetSection("AppParameters"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppParameters:JwtSecretKey"])),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                //TODO - Adicionar LOG quando falhar um Authorization
                var teste = context.Exception.Message;
                return Task.CompletedTask;
            },

            OnMessageReceived = context =>
            {
                context.Request.Cookies.TryGetValue("access_token", out var accessToken);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;
            }
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
    policy.WithOrigins("http://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
});

app.UseAuthentication();

app.UseAuthorization();

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

app.Run();
