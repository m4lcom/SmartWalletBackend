using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SmartWallet.Infrastructure.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SmartWallet API",
                    Version = "v1",
                    Description = "API para gestión de SmartWallet: usuarios, transacciones y servicios externos.",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipo SmartWallet"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // ✅ Evita conflictos de tipos con el mismo nombre
                c.CustomSchemaIds(type => type.FullName);

                // --- definición de seguridad ---
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese 'Bearer' [espacio] y luego su token JWT."
                });

                // --- requerimiento de seguridad global ---
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}

