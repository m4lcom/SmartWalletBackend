# SmartWallet

Billetera virtual desarrollada como proyecto final de **Programación 4 - UTN Rosario**.  
Enfoque en **clean architecture**, **buenas prácticas** y **documentación viva** para asegurar escalabilidad y trazabilidad.

---

## 🚀 Tecnologías principales
- **.NET 8** con **Entity Framework Core**
- **SQLite** (desarrollo) / **SQL Server** (producción)
- **Swagger** para documentación de la API
- **GIT** para el control de versiones y el trabajo colaborativo

---

## 📂 Estructura del proyecto
- `/src` → Código fuente organizado en capas
- `/docs` → Documentación técnica modular
- `changelog.md` → Historial de cambios por versión
- `.github` → Plantillas y configuraciones para PRs e issues

---

## 📄 Documentación
Toda nueva feature o cambio debe:
1. Documentarse en `/docs` usando el [template de nuevas features](./docs/templates/new-feature.md)
2. Agregar la entrada correspondiente en [changelog](./docs/changelog.md)
3. Asociarse a su **branch** siguiendo las [convenciones](./docs/conventions.md)
 
---

## 🛠️ Instalación y configuración

### 1. Requisitos previos
- **.NET 8 SDK** o superior → [Descargar](https://dotnet.microsoft.com/en-us/download)
- **SQLite** instalado
- **Git** para clonar el repositorio → [Descargar](https://git-scm.com/downloads)

### 2. Clonar el repositorio
```bash
  git clone https://github.com/m4lcom/SmartWalletBackend
  cd SmartWallet
```

### 3. Restaurar dependencias
```bash
  dotnet restore
```

### 4. Configurar base de datos
- Verificar cadena de conexión en appsettings.Development.json
- Ejecutar migraciones iniciales
```bash
  dotnet ef database update
```

### 5. Levantar el proyecto en local
```bash
  dotnet run --project src/SmartWallet.Api
```
- La API estará disponible en:
  - Swagger UI: https://localhost:5001/swagger
  - API REST: https://localhost:5001

### 6. Flujo recomendado para desarrollo
  1. Crear una nueva rama desde develop
  2. Implementar la feature/fix/docs
  3. Documentar en /docs usando el template
  4. Actualizar changelog.md
  5. Crear Pull Request hacia develop

---

## Diagrama rápido de arquitectura limpia

```
┌────────────────────┐
│    Presentation    │ → Controllers, DTOs, Swagger
└────────────────────┘
         │
         ▼
┌────────────────────┐
│    Application     │ → Use cases, interfaces, validations
└────────────────────┘
         │
         ▼
┌────────────────────┐
│   Infrastructure   │ → EF Core, repositories, external services
└────────────────────┘
         │
         ▼
┌────────────────────┐
│      Domain        │ → Entities, business logic
└────────────────────┘
```

---

## Equipo
- Bison Julian
- Foca Malcom
- Natale Ezequiel

