# Setup inicial � SmartWallet

Pasos para configurar el entorno de desarrollo y levantar el backend.

---

## 1. Creaci�n de la soluci�n y proyectos
```bash
dotnet new sln -n SmartWalletBackend
dotnet new webapi -n SmartWallet.API
dotnet new classlib -n SmartWallet.Application
dotnet new classlib -n SmartWallet.Domain
dotnet new classlib -n SmartWallet.Infrastructure
```

---

## 2. Agregar proyectos a la soluci�n
```bash
dotnet sln add SmartWallet.API SmartWallet.Application SmartWallet.Domain SmartWallet.Infrastructure
```

---

## 3. Configurar referencias entre proyectos
```bash
dotnet add SmartWallet.API reference SmartWallet.Application SmartWallet.Infrastructure
dotnet add SmartWallet.Infrastructure reference SmartWallet.Domain
dotnet add SmartWallet.Application reference SmartWallet.Domain
```

---

## 4. Instalar paquetes NuGet necesarios
```bash
dotnet add SmartWallet.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite
dotnet add SmartWallet.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add SmartWallet.API package Swashbuckle.AspNetCore
dotnet add SmartWallet.API package Microsoft.EntityFrameworkCore.Design
```

---

## 4. Notas importantes
- Requiere .NET 8 o superior.
- Verificar ruta de la base de datos SQLite.