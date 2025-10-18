using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstraction;
using SmartWallet.Application.Abstractions;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Application.Services;
using SmartWallet.Infrastructure;
using SmartWallet.Infrastructure.Persistence.Repositories;


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
builder.Services.AddSwaggerGen();

// --- applicacion e infrastructure ---
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionLedgerRepository, TransactionLedgerRepository>();
builder.Services.AddScoped<ITransactionLedgerService, TransactionLedgerService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();
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
app.UseAuthorization(); 
app.MapControllers();

app.Run();