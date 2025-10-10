# Changelog – SmartWallet

---

## 2025-08-22
### model project-changelog
Este archivo registra, en orden cronológico, los cambios relevantes del proyecto.  
Cada entrada debe incluir:

- Fecha en formato: `YYYY-MM-DD` 
- Descripción breve  
- Enlace a documentacion tecnica, si lo requiere: 

---

## 2025-08-22
### setup/project-initialization
- Instalación de paquetes básicos.
- Creación del proyecto **SmartWallet** con estructura base de solución y proyectos.

- [Documentación de setup](00-setup.md)  
- [Documentación de arquitectura](01-architecture.md)

---

## 2025-08-31
### config/project-structure-and-config-environment
- Regeneración de `SmartWalletBackend.sln` con rutas físicas correctas.
- Inclusión de archivos raíz como Solution Items: `.env`, `.gitignore`, `README.md`.
- Incorporación de todos los archivos de `/docs/` como Solution Items bajo la carpeta lógica `docs`.
- Eliminación de referencias fantasma y sincronización entre estructura física y lógica.
- Ajuste de `launchSettings.json` para abrir Swagger automáticamente.
- Implementación de carga automática de `.env` usando `Env.TraversePath().Load()` para evitar rutas relativas frágiles.
- Validación de variables críticas (`JWT_SECRET`, `DB_PATH`) antes de iniciar la aplicación.
- Actualización de `.env` con claves y rutas necesarias para la ejecución local.
- Refactor de `appsettings.json` para usar variables de entorno y configuración modular.
- Limpieza y alineación de configuración entre `.env` y `appsettings.json` para mayor portabilidad y reproducibilidad.

- [Convenciones de documentación y Solution Items](conventions.md)  
- [Configuración de entorno y variables](environment.md)  

---

## 2025-09-08
### feature/domain-base-models
Implementar la capa de dominio de SmartWallet con validaciones básicas usando DataAnnotations, sin introducir dependencias de infraestructura.
- Creacion `Domain/Entities/User.cs`  
   - Añadido `[Key]`, `[Required]`, `[StringLength]` y `[EmailAddress]` a propiedades  
   - Relación 1:1 con `Wallet`  
- Creacion `Domain/Entities/Wallet.cs`  
   - Validaciones para `Name`, `CurrencyCode`, `Alias`, `Balance`, `CreatedAt`  
   - Usar `[Column(TypeName = "decimal(18,2)")]` para `Balance`  
- Creacion `Domain/Entities/Transaction.cs`  
   - Validar `Amount` con `[Range(0.01, double.MaxValue)]` y precisión decimal.

---

## 2025-09-08
### agregado de documentacion feature/domain-base-models
- Documentación de la feature **domain-base-model** en `/docs/02-domain-base-model.md`:
  - Objetivo de la feature y alcance.
  - Pasos de implementación con detalle de cambios en `User`, `Wallet` y `Transaction`.
  - Cambios previstos en base de datos.
  - Ejemplos de uso en código.

---

## 2025-09-23
### feature/domain-transaction-ledger-and-status
Implementación de TransactionLedger, enums de TransactionType, TransactionStatus y CurrencyCode, junto con métodos de dominio para control de estados.
- Creación de Domain/Entities/TransactionLedger.cs
	- Registro auditable de operaciones con Timestamp, Type, Amount, CurrencyCode, SourceWalletId, DestinationWalletId, SourceTransactionId, DestinationTransactionId, Status y Metadata.
	- Métodos de fábrica:
		- CreateDeposit
		- CreateWithdrawal
		- CreateTransfer
	- Métodos de dominio con validaciones de transición:
		- MarkAsPending()
		- MarkAsCompleted()
		- MarkAsFailed()
		- MarkAsCanceled()
- Actualización de `TransactionType` en `SmartWallet.Domain.Enums`:
	- Deposit = 0
	- Withdrawal = 1
	- Transfer = 2
- Actualización de `TransactionStatus` en `SmartWallet.Domain.Enums`:
	- Pending = 0
	- Completed = 1
	- Failed = 2
	- Canceled = 3

- [Ciclo de vida de las transacciones](/docs/06-transaction-lifecycle.md).

---

## 2025-10-10
### feature/user-management
- Documentación completa de la arquitectura de gestión de usuarios en `08-user-management.md`.
- Implementación de `UserRepository` con método `GetUserByEmail`.
- Integración de repositorio, servicio y controlador para operaciones CRUD de usuario.
## 2025-09-30
### feature/dbcontext-setup
- Creación de `SmartWalletDbContext` en `Infrastructure.Persistence.Context`.
- Constructor con `DbContextOptions`.
- Método `OnModelCreating` presente, preparado para configuración de relaciones.
- Inclusión de `DbSet` comentados para que el equipo agregue sus entidades.
- Registro del contexto en `Program.cs` usando `UseSqlite` y cadena de conexión desde `.env`.
- Validación de variables críticas (`JWT_SECRET`, `DB_PATH`) antes de iniciar la aplicación.

- [Configuracion de db context ](03-dbcontext-setup.md)

---

## 2025-10-07
### feature/transaction-ledger-vertical-implementation
Implementación completa de la vertical `Transaction` y `TransactionLedger`, incluyendo entidades, repositorios, servicio de aplicación y endpoints de API.

- Creación de `Domain/Entities/Transaction.cs` con propiedades: `Id`, `WalletId`, `Type`, `Amount`, `CurrencyCode`, `Status`, `Date`.
- Creación de `Domain/Entities/TransactionLedger.cs` con propiedades: `Id`, `TransactionId`, `SourceWalletId`, `DestinationWalletId`, `Amount`, `Date`.
- Registro de enums `TransactionType`, `TransactionStatus`, `CurrencyCode` en `SmartWallet.Domain.Enums`.
- Implementación de `TransactionService` con métodos:
  - `DepositAsync`
  - `WithdrawAsync`
  - `TransferAsync`
- Definición de la interfaz `ITransactionService` y registro en `Program.cs`.
- Implementación de `TransactionRepository` y `TransactionLedgerRepository` en `Infrastructure.Persistence.Repositories`.
- Registro de ambos repositorios en el contenedor de dependencias.
- Definición de endpoints en `TransactionsController`:
  - `POST /api/transactions/deposit`
  - `POST /api/transactions/withdraw`
  - `POST /api/transactions/transfer`
- Testeo de endpoints vía Postman con valores válidos (`CurrencyCode = 32`).
- Resolución de errores 405 y 400 relacionados con registro de servicios y validación de enums.
- Documentación técnica de la verticalidad y flujo de orquestación entre entidades.
- Pendiente: integración con `WalletRepository` para validación de saldo y actualización de balance.

- [Documentación de verticalidad Transaction + Ledger](docs/07-transaction-ledger-vertical.md)
