# Gestión de Usuarios (User Management)

## Objetivo
Documentar la implementación actual de la gestión de usuarios en SmartWallet, reflejando el código real de `UserController` y `UserServices`.

---

## Componentes

### 1. Repositorio de Usuario (`IUserRepository` / `UserRepository`)
Responsable del acceso a datos de la entidad `User`. Desacopla la persistencia de la lógica de negocio y expone métodos asincrónicos usados por la capa de servicios.

Firmas principales (implementación esperada en la capa de persistencia):
- `Task<IEnumerable<User>> GetAllAsync()`
- `Task<User?> GetByIdAsync(Guid id)`
- `Task<User?> GetUserByEmailAsync(string email)`
- `Task<bool> CreateAsync(User user)` (la implementación actual devuelve bool)
- `Task UpdateAsync(User user)`
- `Task DeleteAsync(User user)` (usada internamente en rollback)

Ubicación:
- `src/SmartWallet.Application/Abstractions/IUserRepository.cs`
- `src/SmartWallet.Infrastructure/Persistence/Repositories/UserRepository.cs`

---

### 2. Servicio de Usuario (`IUserServices` / `UserServices`)
Interfaz pública:

```csharp
public interface IUserServices
{
    Task<List<UserResponse>> GetAllUsers();
    Task<UserResponse?> GetUserById(Guid id);
    Task<UserResponse?> GetUserByEmail(string email);
    Task<bool> RegisterUser(UserCreateRequest request);
    Task<bool> CreateUser(UserCreateRequest request);
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
- `Task<bool> RegisterUser(UserCreateRequest request)`
- `Task<bool> CreateUser(UserCreateRequest request)`
- `Task<bool> UpdateUser(Guid id, UserUpdateDataRequest request)`
- `Task<bool> ChangeUserActiveStatus(Guid id)`
- `Task<bool> DeleteUser(Guid id)`

**Cambios y comportamientos relevantes:**
- Todos los métodos son asincrónicos (`Task<...>`).
- `RegisterUser` crea un usuario con rol por defecto `Regular` y valida que el email no exista.
- `CreateUser` permite crear usuarios asignando rol explícito (usado por APIs internas o admin).
- `UpdateUser` actualiza campos permitidos (nombre, contraseña, rol, active) usando `Guid id`.
- `ChangeUserActiveStatus` alterna el flag `Active`.
- `DeleteUser` realiza una baja lógica estableciendo `Active = false` y persistiendo el cambio.

Implementación (`UserServices`) — comportamientos clave:
- Mapea entidades `User` a `Contracts.Responses.UserResponse` para los getters (`GetAllUsers`, `GetUserById`, `GetUserByEmail`).
- `RegisterUser(UserRegisterRequest)`:
  - Valida unicidad de email mediante `_userRepository.GetUserByEmailAsync`.
  - Crea una entidad `User` con rol `Regular` y persiste con `_userRepository.CreateAsync`.
  - Crea una wallet por defecto (vía `_walletService.CreateAsync`) si el rol no es `Admin`.
  - Si la creación de wallet falla, intenta un rollback simple eliminando el usuario creado (`_userRepository.DeleteAsync`) y devuelve `false`.
- `CreateAdminUser(UserCreateRequest)`:
  - Similar a `RegisterUser` pero permite crear con rol `Admin` (método público usado por administradores).
  - También contiene la lógica de wallet (se crea sólo si el rol no es `Admin`) y rollback en fallo.
- `UpdateUser(Guid id, UserUpdateDataRequest)`:
  - Recupera el usuario; actualiza solo campos permitidos: `Name`, `Password` y `Active` (si se provee).
  - Persiste con `_userRepository.UpdateAsync`.
- `ChangeUserActiveStatus(Guid id)`:
  - Alterna el flag `Active` y persiste.
- `DeleteUser(Guid id)`:
  - Realiza baja lógica: marca `Active = false` y persiste.

Detalles adicionales:
- `UserServices` usa `IUserRepository` y `IWalletService`.
- El método auxiliar `GenerateAlias(string name, string email)` genera un alias válido (6-20 caracteres, sólo letras y puntos), normaliza acentos, sustituye espacios por puntos, elimina caracteres inválidos y ajusta longitud. Se usa al crear wallets.

Ubicación:
- `src/SmartWallet.Application/Services/IUserServices.cs`
- `src/SmartWallet.Application/Services/UserServices.cs`

---

### 3. Controlador de Usuario (`UserController`)
Atributos de la clase:
- `[Route("api/[controller]")]`
- `[ApiController]`
- `[Authorize]` — exige token por defecto; se anulan acciones públicas con `[AllowAnonymous]` y se restringen otras con `[Authorize(Roles = "Admin")]`.

Helpers internos:
- `GetUserIdFromToken()` — extrae claim `sub` o `NameIdentifier` y lo parsea a `Guid?`.
- `GetUserEmailFromToken()` — extrae claim `email`.
- `IsAdmin()` — comprueba `User.IsInRole("Admin")` o el claim `"role" == "Admin"` (case-insensitive).

Endpoints (rutas y comportamiento exacto según el código actual):

- GET `/api/User`
  - Atributos: `[HttpGet]` + `[Authorize(Roles = "Admin")]`
  - Descripción: Lista todos los usuarios.
  - Comportamiento: llama `await _userServices.GetAllUsers()` y devuelve `200 OK` con la lista.

- GET `/UserById/{userId}` (ruta absoluta)
  - Atributo: `[HttpGet("/UserById/{userId}")]`
  - Autorización: el usuario debe ser Admin o el mismo usuario identificado por el token.
  - Comportamiento:
    - Valida autorización comparando `GetUserIdFromToken()` con `userId` o `IsAdmin()`.
    - Llama `await _userServices.GetUserById(userId)`.
    - Respuestas: `200 OK` con `UserResponse`, `404 NotFound` si no existe, `403 Forbid` si no está autorizado.

- GET `/UserByEmail/{email}` (ruta absoluta)
  - Atributo: `[HttpGet("/UserByEmail/{email}")]`
  - Autorización: Admin o el propio email del token.
  - Comportamiento similar a GetUserById usando `GetUserEmailFromToken()` y `_userServices.GetUserByEmail(email)`.

- POST `/RegisterUser` (ruta absoluta, público)
  - Atributos: `[AllowAnonymous]` + `[HttpPost("/RegisterUser")]`
  - Cuerpo: `UserRegisterRequest`
  - Comportamiento:
    - Llama `await _userServices.RegisterUser(request)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.

- POST `/CreateUser` (ruta absoluta, admin)
  - Atributos: `[Authorize(Roles = "Admin")]` + `[HttpPost("/CreateUser")]`
  - Cuerpo: `UserCreateRequest`
  - Comportamiento:
    - Llama `await _userServices.CreateAdminUser(request)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.

- PUT `/api/User/{id}`
  - Atributo: `[HttpPut("{id}")]`
  - Autorización: solo el propio usuario (se compara `GetUserIdFromToken()` con `id`).
  - Cuerpo: `UserUpdateDataRequest`
  - Comportamiento:
    - Si el token no coincide, devuelve `403 Forbid`.
    - Llama `await _userServices.UpdateUser(id, request)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` si el servicio devuelve `false`.

- PUT `/ChangeActiveStatus/{id}` (ruta absoluta, admin)
  - Atributos: `[Authorize(Roles = "Admin")]` + `[HttpPut("/ChangeActiveStatus/{id}")]`
  - Comportamiento:
    - Llama `await _userServices.ChangeUserActiveStatus(id)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.

- DELETE `/api/User?id={id}`
  - Atributo: `[HttpDelete]` (usa query param)
  - Autorización: Admin o propio usuario (se compara `GetUserIdFromToken()` con `id`).
  - Comportamiento:
    - Llama `await _userServices.DeleteUser(id)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo, `403 Forbid` si no autorizado.

Notas sobre respuestas:
- El controlador actual usa mayormente `Ok()` y `BadRequest()`/`NotFound()`/`Forbid()` según el resultado booleano o la existencia del recurso. No se usa `CreatedAtAction` en la implementación actual (aunque sería apropiado en endpoints de creación).

---

## Resumen del flujo real (según implementación)
1. Petición HTTP -> `UserController` (autoriza por claims según el endpoint).
2. Controller extrae claims si necesita autorizar por usuario/email.
3. Controller llama al `IUserServices` correspondiente (métodos async).
4. `UserServices` aplica reglas de negocio, valida unicidad, mapea entidades y orquesta creación de wallets.
5. `UserServices` persiste a través de `IUserRepository`.
6. Controller traduce el resultado (bool / objeto) a respuesta HTTP (`Ok`, `BadRequest`, `NotFound`, `Forbid`).
