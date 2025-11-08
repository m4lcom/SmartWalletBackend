using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartWallet.Application.Abstractions;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Application.Services;
using SmartWallet.Infrastructure.ExternalServices;
using SmartWallet.Infrastructure.Persistence.Repositories;
using SmartWallet.Infrastructure.ExternalServices.Polly;
using System;
using System.Text;
using AuthenticationService = SmartWallet.Infrastructure.ExternalServices.AuthenticationService;
using IAuthenticationService = SmartWallet.Application.Abstractions.IAuthenticationService;


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

        // --- Servicio de Autenticación ---
        services.AddJwtAuthentication(configuration);
        
        // ---Servicios Externos
        services.AddDolarApi(configuration);

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

    public static IServiceCollection AddDolarApi(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.Configure<DolarApiOptions>(configuration.GetSection("DolarApi"));
        services.Configure<ApiClientConfiguration>(configuration.GetSection("DolarApi:Polly"));

        services.AddHttpClient<IDolarApiService, DolarApiService>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<DolarApiOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                throw new InvalidOperationException("DolarApiOptions.BaseUrl no puede estar vacío. Configure la sección 'DolarApi' en la configuración.");
            }

            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
        })
        
        .AddPolicyHandler((sp, _) =>
        {
            var cfg = sp.GetRequiredService<IOptions<ApiClientConfiguration>>().Value;
            return PollyResiliencePolicies.GetRetryPolicy(cfg);
        })
        .AddPolicyHandler((sp, _) =>
        {
            var cfg = sp.GetRequiredService<IOptions<ApiClientConfiguration>>().Value;
            return PollyResiliencePolicies.GetCircuitBreakerPolicy(cfg);
        });

        return services;
    }
}
