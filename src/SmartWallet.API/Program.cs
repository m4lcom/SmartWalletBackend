using SmartWallet.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- validar configuración ---
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? builder.Configuration["Jwt:Key"];

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- setear variables en configuracion ---
builder.Configuration["Jwt:Key"] = jwtSecret;
builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// --- servicios ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSmartWalletInfrastructure(builder.Configuration);

var app = builder.Build();

// --- validar configuración ---
if (app.Environment.IsProduction())
{
    if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Faltan variables en configuración: Jwt:Key o ConnectionStrings:DefaultConnection.");
}

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

