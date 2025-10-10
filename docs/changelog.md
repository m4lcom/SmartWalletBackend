# Changelog � SmartWallet

---

## 2025-08-22
### model project-changelog
Este archivo registra, en orden cronol�gico, los cambios relevantes del proyecto.  
Cada entrada debe incluir:

- Fecha en formato: `YYYY-MM-DD` 
- Descripci�n breve  
- Enlace a documentacion tecnica, si lo requiere: 

---

## 2025-08-22
### setup/project-initialization
- Instalaci�n de paquetes b�sicos.
- Creaci�n del proyecto **SmartWallet** con estructura base de soluci�n y proyectos.

  - [Documentaci�n de setup](00-setup.md)  
  - [Documentaci�n de arquitectura](01-architecture.md)

---

## 2025-08-31
### config/project-structure-and-config-environment
- Regeneraci�n de `SmartWalletBackend.sln` con rutas f�sicas correctas.
- Inclusi�n de archivos ra�z como Solution Items: `.env`, `.gitignore`, `README.md`.
- Incorporaci�n de todos los archivos de `/docs/` como Solution Items bajo la carpeta l�gica `docs`.
- Eliminaci�n de referencias fantasma y sincronizaci�n entre estructura f�sica y l�gica.
- Ajuste de `launchSettings.json` para abrir Swagger autom�ticamente.
- Implementaci�n de carga autom�tica de `.env` usando `Env.TraversePath().Load()` para evitar rutas relativas fr�giles.
- Validaci�n de variables cr�ticas (`JWT_SECRET`, `DB_PATH`) antes de iniciar la aplicaci�n.
- Actualizaci�n de `.env` con claves y rutas necesarias para la ejecuci�n local.
- Refactor de `appsettings.json` para usar variables de entorno y configuraci�n modular.
- Limpieza y alineaci�n de configuraci�n entre `.env` y `appsettings.json` para mayor portabilidad y reproducibilidad.

  - [Convenciones de documentaci�n y Solution Items](conventions.md)  
  - [Configuraci�n de entorno y variables](environment.md)  

---

## 2025-09-08
### feature/domain-base-models
Implementar la capa de dominio de SmartWallet con validaciones b�sicas usando DataAnnotations, sin introducir dependencias de infraestructura.
- Creacion `Domain/Entities/User.cs`  
   - A�adido `[Key]`, `[Required]`, `[StringLength]` y `[EmailAddress]` a propiedades  
   - Relaci�n 1:1 con `Wallet`  
- Creacion `Domain/Entities/Wallet.cs`  
   - Validaciones para `Name`, `CurrencyCode`, `Alias`, `Balance`, `CreatedAt`  
   - Usar `[Column(TypeName = "decimal(18,2)")]` para `Balance`  
- Creacion `Domain/Entities/Transaction.cs`  
   - Validar `Amount` con `[Range(0.01, double.MaxValue)]` y precisi�n decimal.

---

## 2025-09-08
### agregado de documentacion feature/domain-base-models
- Documentaci�n de la feature **domain-base-model** en `/docs/02-domain-base-model.md`:
  - Objetivo de la feature y alcance.
  - Pasos de implementaci�n con detalle de cambios en `User`, `Wallet` y `Transaction`.
  - Cambios previstos en base de datos.
  - Ejemplos de uso en c�digo.

---

## 2025-09-23
### feature/domain-transaction-ledger-and-status
Implementaci�n de TransactionLedger, enums de TransactionType, TransactionStatus y CurrencyCode, junto con m�todos de dominio para control de estados.
- Creaci�n de Domain/Entities/TransactionLedger.cs
	- Registro auditable de operaciones con Timestamp, Type, Amount, CurrencyCode, SourceWalletId, DestinationWalletId, SourceTransactionId, DestinationTransactionId, Status y Metadata.
	- M�todos de f�brica:
		- CreateDeposit
		- CreateWithdrawal
		- CreateTransfer
	- M�todos de dominio con validaciones de transici�n:
		- MarkAsPending()
		- MarkAsCompleted()
		- MarkAsFailed()
		- MarkAsCanceled()
- Actualizaci�n de `TransactionType` en `SmartWallet.Domain.Enums`:
	- Deposit = 0
	- Withdrawal = 1
	- Transfer = 2
- Actualizaci�n de `TransactionStatus` en `SmartWallet.Domain.Enums`:
	- Pending = 0
	- Completed = 1
	- Failed = 2
	- Canceled = 3

- [Ciclo de vida de las transacciones](/docs/06-transaction-lifecycle.md).

---

## 2025-10-10
### feature/user-management
- Documentaci�n completa de la arquitectura de gesti�n de usuarios en `08-user-management.md`.
- Implementaci�n de `UserRepository` con m�todo `GetUserByEmail`.
- Integraci�n de repositorio, servicio y controlador para operaciones CRUD de usuario.
