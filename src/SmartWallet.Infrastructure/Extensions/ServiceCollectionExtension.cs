using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartWallet.Application.Abstractions;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Application.Services;
using SmartWallet.Infrastructure.Persistence.Repositories;
using System.Text;
using IAuthenticationService = SmartWallet.Application.Abstractions.IAuthenticationService;
using AuthenticationService = SmartWallet.Infrastructure.ExternalServices.AuthenticationService;


namespace SmartWallet.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartWalletInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // --- Repositorios ---
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ITransactionLedgerRepository, TransactionLedgerRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // --- Servicios de aplicación ---
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransactionLedgerService, TransactionLedgerService>();
        services.AddScoped<IUserServices, UserServices>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // --- DbContext ---
        services.AddDbContext<SmartWalletDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                )
            };
        });

        return services;
    }
}
