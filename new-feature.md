# [Nombre de la funcionalidad]

## Objetivo
Breve descripción del propósito de la feature.  
Ejemplo: "Implementar registro y autenticación de usuarios mediante JWT."

---

## Pasos de implementación
1. Resumen de cambios clave en código.
2. Archivos o carpetas creadas/modificadas.
3. Dependencias o paquetes añadidos.

---

## Endpoints
| Método | Ruta | Descripción | Autenticación |
|--------|------|-------------|---------------|
| POST   | /api/auth/register | Registro de un nuevo usuario | ❌ |
| POST   | /api/auth/login    | Login y emisión de token JWT | ❌ |
| GET    | /api/users/me      | Datos del usuario autenticado | ✅ |

---

## Cambios en base de datos
- Migración creada: `YYYYMMDDHHmm_add_auth_tables`
- Tablas nuevas o modificadas:
  - `Users` (campos: `Id`, `Username`, `PasswordHash`, ...)
  - `Roles` (opcional)

---

## Ejemplos de uso
```bash
curl -X POST https://localhost:5001/api/auth/register \
-H "Content-Type: application/json" \
-d '{"username":"demo","password":"1234"}'
