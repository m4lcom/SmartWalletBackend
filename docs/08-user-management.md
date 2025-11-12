# Gestión de Usuarios (User Management)

## Objetivo
Documentar el estado actualizado y real del subsistema de gestión de usuarios en SmartWallet, alineado con las implementaciones actuales de `UserController`, `UserServices` e `UserRepository`.

---

## Resumen ejecutivo
- Las operaciones de creación públicas devuelven ahora el recurso creado (no sólo un booleano).
- `RegisterUser` devuelve `UserwithWalletResponse?` incluyendo la wallet creada cuando aplica.
- Las operaciones de actualización y cambio de estado devuelven el `UserResponse` actualizado.
- El repositorio usa EF Core y `GetUserByEmailAsync` aplica `AsNoTracking()` para lecturas/validaciones.
- Las políticas de autorización (`SameUserOrAdmin`) y roles (`Admin`) siguen gobernando el acceso a endpoints.

---

## 1) Repositorio de Usuario (`IUserRepository` / `UserRepository`)
Responsabilidad: acceso a datos de la entidad `User` sobre EF Core.

Firmas relevantes (implementación actual):
- `Task<List<User>> GetAllAsync()`
- `Task<User?> GetByIdAsync(Guid id)`
- `Task<User?> GetUserByEmailAsync(string email)` — implementado con `AsNoTracking()` para lecturas de verificación.
- `Task<bool> CreateAsync(User user)` — devuelve `bool` indicando éxito.
- `Task<bool> UpdateAsync(User user)` — devuelve `bool` indicando éxito.
- `Task<bool> DeleteAsync(User user)` — devuelve `bool` indicando éxito (se usa en rollback).

Ubicación:
- `src/SmartWallet.Application/Abstractions/IUserRepository.cs`
- `src/SmartWallet.Infrastructure/Persistence/Repositories/UserRepository.cs`

Notas:
- `AsNoTracking()` evita overhead de change tracking cuando sólo se valida existencia o se lee para respuesta.
- `BaseRepository` proporciona implementaciones genéricas (`CreateAsync`, `UpdateAsync`, `DeleteAsync`) que devuelven `bool`.

---

## 2) Servicio de Usuario (`IUserServices` / `UserServices`)
Interfaz pública actual (firmas principales):
```csharp
public interface IUserServices
{
    Task<List<UserResponse>> GetAllUsers();
    Task<UserResponse?> GetUserById(Guid id);
    Task<UserResponse?> GetUserByEmail(string email);
    Task<UserwithWalletResponse?> RegisterUser(UserCreateRequest request);
    Task<UserResponse?> CreateUser(UserCreateRequest request);
    Task<bool> UpdateUser(Guid id, UserUpdateDataRequest request);
    Task<bool> ChangeUserActiveStatus(Guid id);
    Task<bool> DeleteUser(Guid id);
}
```
**Responsabilidades:**
- Exponer operaciones asincrónicas de alto nivel para la gestión de usuarios.
- Validar reglas de negocio (p. ej. unicidad de email) antes de persistir.
- Mapear entidades de dominio a `Contracts.Responses.UserResponse` y recibir `Contracts.Requests.*`.

**Firmas principales (implementación actual):**
- `Task<List<UserResponse>> GetAllUsers()`
- `Task<UserResponse?> GetUserById(Guid id)`
- `Task<UserResponse?> GetUserByEmail(string email)`
- `Task<UserwithWalletResponse?> RegisterUser(UserCreateRequest request)`
- `Task<UserResponse?> CreateUser(UserCreateRequest request)`
- `Task<bool> UpdateUser(Guid id, UserUpdateDataRequest request)`
- `Task<bool> ChangeUserActiveStatus(Guid id)`
- `Task<bool> DeleteUser(Guid id)`

**Cambios y comportamientos relevantes:**
- Todos los métodos son asincrónicos (`Task<...>`).
- `RegisterUser` crea un usuario con rol por defecto `Regular`, valida que el email no exista y devuelve el `UserwithWalletResponse?` con la información del usuario y wallet creados.
- `CreateUser` permite crear usuarios asignando rol explícito y devuelve el `UserResponse?` del usuario creado (usado por APIs internas o admin).
- `UpdateUser` actualiza campos permitidos (nombre, contraseña, rol, active) usando `Guid id` y devuelve el `UserResponse` actualizado.
- `ChangeUserActiveStatus` alterna el flag `Active` y persiste el cambio.
- `DeleteUser` realiza una baja lógica estableciendo `Active = false` y persistiendo el cambio.

Implementación (`UserServices`) — comportamientos clave:
- Mapea entidades `User` a `Contracts.Responses.UserResponse` para los getters (`GetAllUsers`, `GetUserById`, `GetUserByEmail`).
- `RegisterUser(UserRegisterRequest)`:
  - Valida unicidad de email mediante `_userRepository.GetUserByEmailAsync`.
  - Crea una entidad `User` con rol `Regular` y persiste con `_userRepository.CreateAsync`.
  - Crea una wallet por defecto (vía `_walletService.CreateAsync`) si el rol no es `Admin`.
  - Si la creación de wallet falla, intenta un rollback simple eliminando el usuario creado (`_userRepository.DeleteAsync`) y devuelve `null`.
  - Devuelve el `UserwithWalletResponse?` con la información del usuario registrado y la wallet creada.
- `CreateAdminUser(UserCreateRequest)`:
  - Similar a `RegisterUser` pero permite crear con rol `Admin` (método público usado por administradores).
  - También contiene la lógica de wallet (se crea sólo si el rol no es `Admin`) y rollback en fallo.
  - Devuelve el `UserResponse?` con la información del usuario creado.
- `UpdateUser(Guid id, UserUpdateDataRequest)`:
  - Recupera el usuario; actualiza solo campos permitidos: `Name`, `Password` y `Active` (si se provee).
  - Persiste con `_userRepository.UpdateAsync` y devuelve el `UserResponse` actualizado.
- `ChangeUserActiveStatus(Guid id)`:
  - Alterna el flag `Active` y persiste.
- `DeleteUser(Guid id)`:
  - Realiza baja lógica: marca `Active = false` y persiste.

Comportamiento y responsabilidades:
- Validaciones de negocio (p. ej. unicidad de `Email` con `_userRepository.GetUserByEmailAsync`).
- Persiste `Domain.Entities.User` usando `IUserRepository`.
- Orquesta creación de wallet por defecto vía `IWalletService.CreateAsync` cuando el rol no es `Admin`.
- En creación pública (`RegisterUser`) devuelve `UserwithWalletResponse?` que incluye `WalletId` y `WalletAlias` si se creó wallet.
- Si la creación de wallet falla tras crear el usuario, se intenta un rollback simple (`_userRepository.DeleteAsync(newUser)`) y se devuelve `null`.
- `CreateAdminUser` permite crear con rol explícito (devuelve `UserResponse?`).
- `UpdateUser` y `ChangeUserActiveStatus` actualizan la entidad, persisten y devuelven el `UserResponse` actualizado.
- `DeleteUser` realiza baja lógica (`Active = false`) y devuelve `bool` de éxito.

Helpers internos importantes:
- `GenerateAlias(string name, string email)` — genera alias válidos para wallets (6–20 caracteres; sólo letras y puntos).
- `MapToResponse(Domain.Entities.User)` — mapea `User` → `UserResponse`.
- `MapToUserWithWalletResponse(Domain.Entities.User, Guid walletId, string walletAlias)` — mapea `User` + wallet → `UserwithWalletResponse`.

Ubicación:
- `src/SmartWallet.Application/Services/IUserServices.cs`
- `src/SmartWallet.Application/Services/UserServices.cs`

Ejemplo de flujo en `RegisterUser`:
1. Verificar que no exista email.
2. Crear `User` (rol `Regular`) y persistir con `_userRepository.CreateAsync`.
3. Si rol != `Admin`, generar alias y crear wallet con `_walletService.CreateAsync`.
4. Si wallet OK, devolver `UserwithWalletResponse` con `WalletId`/`WalletAlias`.
5. Si la creación de wallet falla, intentar eliminar usuario (rollback) y devolver `null`.

---

## 3) Controlador de Usuario (`UserController`)
Atributos de clase:
- `[Route("api/[controller]")]`
- `[ApiController]`
- Uso de `[Authorize]` global por defecto; endpoints abiertos con `[AllowAnonymous]`; control por roles con `[Authorize(Roles = "Admin")]`.
- Se usa la política `SameUserOrAdmin` para permitir acceso al propio recurso o a administradores.

Rutas y comportamiento actual:

- GET `/api/User`
  - `[Authorize(Roles = "Admin")]`
  - Llama: `_userServices.GetAllUsers()`
  - Respuesta: `200 OK` con `List<UserResponse>`

- GET `/api/User/{userId:guid}`
  - `[Authorize(Policy = "SameUserOrAdmin")]`
  - Llama: `_userServices.GetUserById(userId)`
  - Respuesta: `200 OK` con `UserResponse` o `404 NotFound`

- GET `/api/User/by-email/{email}`
  - `[Authorize(Policy = "SameUserOrAdmin")]`
  - Llama: `_userServices.GetUserByEmail(email)`
  - Respuesta: `200 OK` con `UserResponse` o `404 NotFound`

- POST `/api/User/register` (público)
  - `[AllowAnonymous]`
  - Body: `UserRegisterRequest`
  - Llama: `_userServices.RegisterUser(request)`
  - Respuestas:
    - `201 Created` con `UserwithWalletResponse` (incluye `WalletId`/`WalletAlias`) si tiene éxito.
    - `400 BadRequest` si falla (ej. email duplicado o fallo en wallet).

- POST `/api/User/create` (admin)
  - `[Authorize(Roles = "Admin")]`
  - Body: `UserCreateRequest`
  - Llama: `_userServices.CreateAdminUser(request)`
  - Respuestas:
    - `201 Created` con `UserResponse` si éxito.
    - `400 BadRequest` en fallo.

- PUT `/api/User/{id:guid}`
  - `[Authorize(Policy = "SameUserOrAdmin")]`
  - Body: `UserUpdateDataRequest`
  - Llama: `_userServices.UpdateUser(id, request)`
  - Respuestas:
    - `200 OK` con `UserResponse` actualizado.
    - `400 BadRequest` en fallo.

- PUT `/api/User/{id:guid}/active`
  - `[Authorize(Roles = "Admin")]`
  - Llama: `_userServices.ChangeUserActiveStatus(id)`
  - Respuestas:
    - `200 OK` con `UserResponse` actualizado.
    - `400 BadRequest` en fallo.

- DELETE `/api/User/{id:guid}`
  - `[Authorize(Policy = "SameUserOrAdmin")]`
  - Llama: `_userServices.DeleteUser(id)`
  - Respuestas:
    - `200 OK` si éxito (sin body).
    - `400 BadRequest` en fallo.

Notas sobre estado HTTP:
- Las creaciones usan `CreatedAtAction(nameof(GetUserById), new { userId = id }, response)` para devolver `201 Created` con Location.
- Lecturas y actualizaciones devuelven `200 OK` con el recurso.
- Autorización fallida es gestionada por ASP.NET Core y la política `SameUserOrAdmin` (resulta en `403 Forbidden` cuando aplica).

---

## DTOs / Respuestas relevantes
- `Contracts.Responses.UserResponse` — representación básica del usuario (Id, Name, Email, Role, CreatedAt, UpdatedAt, Active).
- `Contracts.Responses.UserwithWalletResponse` — `UserResponse` ampliado con:
  - `Guid WalletId`
  - `string WalletAlias`
  (usado por `RegisterUser` cuando se crea una wallet por defecto).

Ubicación DTOs:
- `src\Contracts\Responses\UserResponse.cs`
- `src\Contracts\Responses\UserwithWalletResponse.cs`

---

## Flujo completo (resumen)
1. Cliente → `UserController` (autorización por atributos/políticas).
2. `UserController` → `IUserServices` (operación correspondiente).
3. `UserServices`:
   - valida reglas (ej. unicidad email),
   - crea/actualiza `User` y persiste via `IUserRepository`,
   - orquesta `IWalletService` cuando toca,
   - mapea entidad a `UserResponse` o `UserwithWalletResponse`.
4. `IUserRepository` usa EF Core para persistir/consultar; `GetUserByEmailAsync` usa `AsNoTracking()` para lecturas.
5. `UserController` devuelve `201 Created` o `200 OK` con el recurso, o `400/404` según corresponda.

---

## Archivos de interés / ubicación en el repo
- Controlador: `src/SmartWallet.API/Controllers/UserController.cs`
- Servicio e interfaz: `src/SmartWallet.Application/Services/UserServices.cs`, `src/SmartWallet.Application/Services/IUserServices.cs`
- Repositorio: `src/SmartWallet.Infrastructure/Persistence/Repositories/UserRepository.cs`
- DTOs/responses: `src/Contracts/Responses/UserResponse.cs`, `src/Contracts/Responses/UserwithWalletResponse.cs`
- Requests: `src/Contracts/Requests/UserRegisterRequest.cs`, `src/Contracts/Requests/UserCreateRequest.cs`, `src/Contracts/Requests/UserUpdateDataRequest.cs`

---

## Recomendaciones (opcional)
- Considerar renombrar `UserwithWalletResponse` → `UserWithWalletResponse` (PascalCase) y propagar su uso para consistencia.
- Añadir pruebas de integración que verifiquen:
  - `RegisterUser` crea usuario y wallet, y devuelve `UserwithWalletResponse` con `WalletId`.
  - Rollback en fallo de wallet.
- Documentar en OpenAPI/Swagger las respuestas 201 con schema de `UserwithWalletResponse`.