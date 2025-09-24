# Configuracion de entorno

Este documento describe las variables de entorno y los archivos de configuración necesarios para ejecutar SmartWallet de forma local o en diferentes entornos.

---

## 1. Variables de Entorno (`.env`)

| Variable       | Description                                      | Example Value           |
|----------------|--------------------------------------------------|-------------------------|
| `JWT_SECRET`   | Secret key used to sign JWT tokens               | `super-secret-key`      |
| `DB_PATH`      | Path to the SQLite database file                 | `smartwallet.db`        |

> **No debes commitear tu archivo `.env` al repositorio. Debe estar listado en `.gitignore`.

---

## 2. `.env.example`
```
JWT_SECRET=your-secret-key-here 
DB_PATH=smartwallet.db
```
Los desarrolladores deben copiar este archivo a .`env` y reemplazar los valores de ejemplo por los reales.

---

## `appsettings.json`
Ubicado en `src/SmartWalletAPI/` y contiene configuración no sensible:

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

## 4. Carga de Variables de Entorno en `Program.cs`
```
using DotNetEnv;

Env.TraversePath().Load(); // Busca .env hacia arriba

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
var dbPath = Environment.GetEnvironmentVariable("DB_PATH");

if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(dbPath))
    throw new InvalidOperationException("Faltan variables: JWT_SECRET o DB_PATH");

builder.Configuration["Jwt:Key"] = jwtSecret;
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

```

---

## 5. Archivos Específicos por Entorno
- `appsettings.Development.json` → Configuración para desarrollo local
- `appsettings.Staging.json` → Configuración para entorno de pruebas
- `appsettings.Production.json` → Configuración para producción

---

## Notas de Seguridad
Nunca commitear `.env` ni `appsettings` de producción con secretos reales.

Rotar periódicamente la clave `JWT_SECRET`.

Usar claves diferentes para cada entorno.

---



