using SmartWallet.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- validar configuración ---
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? builder.Configuration["Jwt:Key"];
var dbPath = Environment.GetEnvironmentVariable("DB_PATH")
             ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(dbPath))
    throw new InvalidOperationException("Faltan variables en configuración: Jwt:Key o ConnectionStrings:DefaultConnection.");

// --- servicios ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSmartWalletInfrastructure(builder.Configuration);

var app = builder.Build();

// --- pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

