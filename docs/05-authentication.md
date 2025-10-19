# Autenticación y autorización con JWT

## Objetivo
Implementar registro y autenticación de usuarios mediante JWT.  
Permitir que usuarios se registren, inicien sesión y obtengan sus datos autenticados desde un endpoint protegido.

---

## Pasos de implementación
1. **Cambios clave en código**
   - `AuthController`: agrega endpoints de registro (`/register`) y login (`/login`).
   - `AuthenticationService`: valida credenciales y emite tokens JWT con claims (`sub`, `email`, `name`, `role`).
   - `Program.cs`: configuración de `AddAuthentication` con `JwtBearer`, `AddAuthorization` y Swagger con esquema Bearer.
   - `UserController`: incorpora `/users/me` para devolver datos del usuario autenticado (requiere JWT).
   - Se agregaron `[Authorize]` y restricciones por rol en controllers sensibles (`Transactions`, `TransactionLedgers`, `User`).

2. **Archivos o carpetas creadas/modificadas**
   - `Contracts/Requests/LoginRequest.cs`
   - `Contracts/Requests/UserCreateRequest.cs`
   - `Application/Abstractions/IAuthenticationService.cs`
   - `Infrastructure/ExternalServices/AuthenticationService.cs`
   - `Infrastructure/Persistence/Repositories/UserRepository.cs`
   - `API/Controllers/AuthController.cs`
   - `API/Controllers/UserController.cs`
   - `Program.cs`

3. **Dependencias o paquetes añadidos**
   - `Microsoft.AspNetCore.Authentication.JwtBearer`
   - `System.IdentityModel.Tokens.Jwt`
   - `Microsoft.IdentityModel.Tokens`

---

## Endpoints
| Método | Ruta                | Descripción                       | Autenticación |
|--------|---------------------|-----------------------------------|---------------|
| POST   | /api/auth/register  | Registro de un nuevo usuario      | ❌ |
| POST   | /api/auth/login     | Login y emisión de token JWT      | ❌ |
| GET    | /api/users/me       | Datos del usuario autenticado     | ✅ |

---

## Cambios en base de datos
- **Migración creada:** `202510182100_add_auth_tables`
- **Tablas nuevas o modificadas:**
  - `Users`  
    - `Id` (PK, Guid)  
    - `Name` (string)  
    - `Email` (string, único)  
    - `PasswordHash` (string)  
    - `Role` (enum/string)  
    - `Active` (bool)  
    - `CreatedAt`, `UpdatedAt` (DateTime)  
  - `Roles` (opcional, si se maneja como tabla separada)

---

## Ejemplos de uso
```bash
# Registro
curl -X POST https://localhost:5001/api/auth/register \
-H "Content-Type: application/json" \
-d '{"name":"demo","email":"demo@demo.com","password":"1234","role":"User"}'

# Login
curl -X POST https://localhost:5001/api/auth/login \
-H "Content-Type: application/json" \
-d '{"email":"demo@demo.com","password":"1234"}'

# Usar el token recibido para acceder a /users/me
curl -X GET https://localhost:5001/api/users/me \
-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
