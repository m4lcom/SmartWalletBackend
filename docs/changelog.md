# Changelog – SmartWallet

Este archivo registra, en orden cronológico, los cambios relevantes del proyecto.  
Cada entrada debe incluir:

- Fecha en formato: `YYYY-MM-DD` 
- Descripción breve  
- Enlace a documentacion tecnica, si lo requiere: 

---

## 2025-08-22
- Instalación de paquetes básicos.
- Creación del proyecto **SmartWallet** con estructura base de solución y proyectos.

  - [Documentación de setup](00-setup.md)  
  - [Documentación de arquitectura](01-architecture.md)

---

## 2025-08-31
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