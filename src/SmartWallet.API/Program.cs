using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SmartWallet.Infrastructure.Persistence.Context;
using System;
using SmartWallet.Application.Interfaces;
using SmartWallet.Application.Services;
using SmartWallet.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


Env.TraversePath().Load(); // Usamos TraversePath() para evitar rutas relativas al cargar .env

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
var dbPath = Environment.GetEnvironmentVariable("DB_PATH");

if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(dbPath))
{
    throw new InvalidOperationException($"Faltan variables en .env: JWT_SECRET o DB_PATH.");
}

builder.Configuration["Jwt:Key"] = jwtSecret;
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SmartWalletDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<IWalletService, WalletService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();