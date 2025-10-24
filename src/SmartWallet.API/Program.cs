using DotNetEnv;
using SmartWallet.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- cargar .env ---
Env.TraversePath().Load();

// --- validar variables ---
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
var dbPath = Environment.GetEnvironmentVariable("DB_PATH");

if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(dbPath))
    throw new InvalidOperationException("Faltan variables en .env: JWT_SECRET o DB_PATH.");

builder.Configuration["Jwt:Key"] = jwtSecret;
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

// --- servicios ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSmartWalletInfrastructure(builder.Configuration);

var app = builder.Build();

// --- pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
