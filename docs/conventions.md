# Conventions

## Ramas
- `main` → estable
- `develop` → integración
- `feature/*` → nuevas funcionalidades
- `bugfix/*` → correcciones
- `refactor/*` → reestructuración de código
- `chore/*` → soporte, mantenimiento, tareas varias
- `docs/*` → cambios de documentación

---

## Commits
- `feat:` nueva funcionalidad  
- `fix:` corrección de bug  
- `docs:` cambios en documentación  
- `refactor:` reestructuración de código  
- `test:` pruebas  
- `chore:` soporte, tareas varias
- `config:` cambios en configuración de entorno o archivos `appsettings`

---

## Pull Requests
- Descripción clara.
- Enlace a tarea o documentación relacionada.

---

## Solution Items y Documentación

- Todos los archivos de `/docs/` deben estar incluidos como Solution Items en Visual Studio bajo una carpeta lógica `docs`.
- Los archivos `.env`, `.gitignore` y `README.md` deben estar incluidos como Solution Items en la raíz de la solución.
- Cualquier cambio en estructura, configuración o documentación debe registrarse en `docs/changelog.md` siguiendo el formato establecido.
- Cada nueva funcionalidad o cambio significativo debe tener su propia documentación en `/docs/` siguiendo el formato de `docs/new-feature.md`.

---

