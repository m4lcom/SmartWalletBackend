using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartWallet.Application.Abstractions;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Application.Services;
using SmartWallet.Infrastructure;
using SmartWallet.Infrastructure.ExternalServices;
using SmartWallet.Infrastructure.Persistence.Repositories;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// --- cargar variables de entorno  ---
Env.TraversePath().Load(); // Usamos TraversePath() para evitar rutas relativas al cargar .env

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// --- validar variables de entorno ---
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
var dbPath = Environment.GetEnvironmentVariable("DB_PATH");

if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(dbPath))
{
    throw new InvalidOperationException($"Faltan variables en .env: JWT_SECRET o DB_PATH.");
}

// --- configurar settings ---
builder.Configuration["Jwt:Key"] = jwtSecret;
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

// --- servicios de la API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SmartWallet API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese 'Bearer' [espacio] y luego su token JWT."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// --- applicacion e infrastructure ---
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionLedgerRepository, TransactionLedgerRepository>();
builder.Services.AddScoped<ITransactionLedgerService, TransactionLedgerService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

// --- registrar Dbcontext --- 
builder.Services.AddDbContext<SmartWalletDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- middleware pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartWallet API v1");
        // options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();   
app.UseAuthorization(); 
app.MapControllers();

app.Run();