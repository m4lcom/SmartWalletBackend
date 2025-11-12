using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartWallet.Application.Abstractions;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Application.Services;
using SmartWallet.Infrastructure.ExternalServices;
using SmartWallet.Infrastructure.ExternalServices.Polly;
using SmartWallet.Infrastructure.Persistence.Repositories;
using SmartWallet.Infrastructure.Services;
using System;
using System.Text;
using AuthenticationService = SmartWallet.Infrastructure.ExternalServices.AuthenticationService;
using IAuthenticationService = SmartWallet.Application.Abstractions.IAuthenticationService;

namespace SmartWallet.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmartWalletInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // --- repositorios ---
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionLedgerRepository, TransactionLedgerRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // --- servicios de aplicación ---
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITransactionLedgerService, TransactionLedgerService>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();


            // --- Autenticación y autorización ---
            services.AddJwtAuthentication(configuration);
            services.AddAuthorizationPolicies();

            // --- swagger ---
            services.AddSwaggerDocumentation();

            // --- servicios externos ---
            services.AddDolarApi(configuration);

            // --- DbContext con resiliencia ---
            services.AddDbContext<SmartWalletDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null
                        );
                    }
                )
            );

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

        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SameUserOrAdmin", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var user = context.User;
                        if (user == null)
                            return false;

                        // Si es admin, permite todo
                        if (user.IsAdmin())
                            return true;

                        Guid tokenUserId;
                        var tokenUserIdNullable = user.GetUserId();
                        tokenUserId = tokenUserIdNullable ?? Guid.Empty;

                        var tokenEmail = user.GetUserEmail();

                        // Extraer RouteValues desde distintos recursos posibles
                        Microsoft.AspNetCore.Routing.RouteValueDictionary? routeValues = null;

                        if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcCtx)
                        {
                            routeValues = mvcCtx.RouteData.Values;
                        }
                        else if (context.Resource is HttpContext httpCtx)
                        {
                            routeValues = httpCtx.Request.RouteValues;
                        }

                        if (routeValues == null)
                            return false;

                        // Comprobar claves comunes para id de usuario
                        if (routeValues.TryGetValue("userId", out var routeUserIdObj)
                            || routeValues.TryGetValue("id", out routeUserIdObj)
                            || routeValues.TryGetValue("userid", out routeUserIdObj)
                            || routeValues.TryGetValue("user_id", out routeUserIdObj))
                        {
                            if (Guid.TryParse(routeUserIdObj?.ToString(), out var routeUserId))
                            {
                                if (tokenUserId != Guid.Empty && tokenUserId == routeUserId)
                                    return true;
                            }
                        }

                        // Comprobar por email en ruta
                        if (routeValues.TryGetValue("email", out var routeEmailObj)
                            || routeValues.TryGetValue("Email", out routeEmailObj))
                        {
                            if (routeEmailObj is string routeEmail
                                && !string.IsNullOrWhiteSpace(tokenEmail)
                                && string.Equals(routeEmail, tokenEmail, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }

                        return false;
                    }));
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
}

