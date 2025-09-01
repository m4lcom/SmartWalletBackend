# Changelog � SmartWallet

Este archivo registra, en orden cronol�gico, los cambios relevantes del proyecto.  
Cada entrada debe incluir:

- Fecha en formato: `YYYY-MM-DD` 
- Descripci�n breve  
- Enlace a documentacion tecnica, si lo requiere: 

---

## 2025-08-22
- Instalaci�n de paquetes b�sicos.
- Creaci�n del proyecto **SmartWallet** con estructura base de soluci�n y proyectos.

  - [Documentaci�n de setup](00-setup.md)  
  - [Documentaci�n de arquitectura](01-architecture.md)

---

## 2025-08-31
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