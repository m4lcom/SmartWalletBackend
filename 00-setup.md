# Setup inicial — SmartWallet

Este documento describe la configuración base del proyecto SmartWallet, para que todos podamos  replicarla desde cero.

---

## 1. Creación de la solución y proyectos
```bash
dotnet new sln -n SmartWallet
dotnet new webapi -n SmartWallet.API
dotnet new classlib -n SmartWallet.Application
dotnet new classlib -n SmartWallet.Domain
dotnet new classlib -n SmartWallet.Infrastructure
```

---

## 2. Agregar proyectos a la solución
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