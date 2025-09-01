# Environment Configuration

This document describes the environment variables and configuration files required to run **SmartWallet** locally or in different environments.

---

## 1. Environment Variables (`.env`)

| Variable       | Description                                      | Example Value           |
|----------------|--------------------------------------------------|-------------------------|
| `JWT_SECRET`   | Secret key used to sign JWT tokens               | `super-secret-key`      |
| `DB_PATH`      | Path to the SQLite database file                 | `smartwallet.db`        |

> **Do not commit** your `.env` file to the repository. It must be listed in `.gitignore`.

---

## 2. `.env.example`
```
JWT_SECRET=your-secret-key-here 
DB_PATH=smartwallet.db
```
Developers should copy this file to `.env` and replace the placeholder values.

---

## 3. `appsettings.json`

Located in `src/SmartWalletAPI/` and contains non-sensitive configuration:

```json
{
  "ConnectionStrings": {
    // Usamos variable de entorno para la ruta de la DB
    "DefaultConnection": "${DB_PATH}"
  },
  "Jwt": {
    // Clave JWT desde .env
    "Key": "${JWT_ST}",
    "Issuer": "SmartWalletAPI",
    "Audience": "SmartWalletClient",
    "ExpireMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",

  // Configuración de entorno
  "Environment": {
    "Name": "Development",
    "UseEnvFile": true,
    "EnvFilePath": ".env"
  },

  // Configuración propia de SmartWallet
  "SmartWallet": {
    "EnableDocs": true,
    "DocsPath": "docs/",
    "ChangelogPath": "docs/changelog.md"
  },

  // Configuración de Swagger
  "Swagger": {
    "Enabled": true,
    "Title": "SmartWallet API",
    "Version": "v1"
  },

  // Features opcionales
  "Features": {
    "EnableEmailNotifications": false,
    "EnableAuditTrail": true
  }
}

```

---

## 4. Loading Environment Variables in `Program.cs`
```
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file from repo root
Env.Load();

// Inject sensitive values into configuration
builder.Configuration["Jwt:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["Database:ConnectionString"] = $"Data Source={Environment.GetEnvironmentVariable("DB_PATH")}";
```

---

## 5. Environment-specific Files
- `appsettings.Development.json` → Local development overrides
- `appsettings.Staging.json` → Staging environment overrides
- `appsettings.Production.json` → Production environment overrides

---

## 6. Security Notes
- Never commit .env or production appsettings with real secrets.
- Rotate JWT_SECRET periodically.
- Use different secrets per environment.

---



