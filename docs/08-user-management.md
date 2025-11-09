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
- `[Authorize]` — exige token por defecto; se anulan acciones públicas con `[AllowAnonymous]` y se restringen acciones administrativas con `[Authorize(Roles = "Admin")]`.

Nota importante: la implementación actual del controlador usa políticas de autorización (`SameUserOrAdmin`) para permitir que un usuario acceda a sus propios recursos o que un Admin tenga acceso; la lógica de comparación de claims (usuario vs recurso) se desplaza a la política, por lo que no hay helpers explícitos para extraer `sub`/`email` dentro del controlador.

Endpoints (rutas y comportamiento según el código actual):

- GET `GET /api/User`
  - Atributos: `[HttpGet]` + `[Authorize(Roles = "Admin")]`
  - Descripción: Lista todos los usuarios.
  - Comportamiento: llama `await _userServices.GetAllUsers()` y devuelve `200 OK` con la lista.

- GET `GET /api/User/{userId}` (route relative)
  - Atributos: `[HttpGet("{userId:guid}")]` + `[Authorize(Policy = "SameUserOrAdmin")]`
  - Autorización: la política `SameUserOrAdmin` autoriza si el usuario es Admin o si el `sub` del token coincide con `userId`.
  - Comportamiento:
    - Llama `await _userServices.GetUserById(userId)`.
    - Respuestas: `200 OK` con `UserResponse`, `404 NotFound` si no existe.
    - La política de autorización se encarga de devolver `403 Forbid` cuando corresponde.

- GET `GET /api/User/by-email/{email}`
  - Atributos: `[HttpGet("by-email/{email}")]` + `[Authorize(Policy = "SameUserOrAdmin")]`
  - Autorización: la política permite acceso si el usuario es Admin o si el claim `email` del token coincide con el parámetro.
  - Comportamiento: similar a GetUserById usando `_userServices.GetUserByEmail(email)`. Respuestas: `200 OK` / `404 NotFound`.

- POST `POST /api/User/register` (público)
  - Atributos: `[AllowAnonymous]` + `[HttpPost("register")]`
  - Cuerpo: `UserRegisterRequest`
  - Comportamiento:
    - Llama `await _userServices.RegisterUser(request)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.

- POST `POST /api/User/create` (admin)
  - Atributos: `[Authorize(Roles = "Admin")]` + `[HttpPost("create")]`
  - Cuerpo: `UserCreateRequest`
  - Comportamiento:
    - Llama `await _userServices.CreateAdminUser(request)` (método concreto en la implementación).
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.

- PUT `PUT /api/User/{id}` (propietario o admin)
  - Atributos: `[HttpPut("{id:guid}")]` + `[Authorize(Policy = "SameUserOrAdmin")]`
  - Cuerpo: `UserUpdateDataRequest`
  - Comportamiento:
    - Llama `await _userServices.UpdateUser(id, request)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` si el servicio devuelve `false`.

- PUT `PUT /api/User/{id}/active` (admin)
  - Atributos: `[HttpPut("{id:guid}/active")]` + `[Authorize(Roles = "Admin")]`
  - Comportamiento:
    - Llama `await _userServices.ChangeUserActiveStatus(id)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.

- DELETE `DELETE /api/User/{id}` (propietario o admin)
  - Atributos: `[HttpDelete("{id:guid}")]` + `[Authorize(Policy = "SameUserOrAdmin")]`
  - Comportamiento:
    - Llama `await _userServices.DeleteUser(id)`.
    - Respuestas: `200 OK` en éxito, `400 BadRequest` en fallo.
    - La política controla `403 Forbid` cuando el solicitante no está autorizado.

Notas sobre respuestas:
- El controlador actualmente usa `Ok()`, `BadRequest()` y `NotFound()` en sus métodos; las denegaciones por autorización dependen de la infraestructura de políticas (`403 Forbid`) aplicada por ASP.NET Core.
- No se usan `CreatedAtAction` en los endpoints de creación en la implementación actual; las respuestas exitosas devuelven `200 OK`.

---

## Resumen del flujo real (según implementación)
1. Petición HTTP -> `UserController` (autoriza por atributos y políticas).
2. Controller delega a `IUserServices` para la lógica de negocio.
3. `UserServices` aplica reglas de negocio, valida unicidad, mapea entidades y orquesta creación de wallets.
4. `UserServices` persiste a través de `IUserRepository`.
5. Controller traduce el resultado (bool / objeto) a respuesta HTTP (`Ok`, `BadRequest`, `NotFound`), y la infraestructura de autorización devuelve `403` cuando aplica.

---

## Cambios principales respecto a la documentación previa
- Rutas: se unifican como rutas relativas bajo `api/User` en lugar de rutas absolutas fuera del prefijo (p. ej. ya no hay `/UserById/{id}` ni `/RegisterUser` directas; ahora son `api/User/{id}` y `api/User/register`).
- Autorización: se utiliza la política `SameUserOrAdmin` para endpoints que permiten acceso al propio usuario o a administradores, en lugar de lógica manual de extracción de claims dentro del controlador.
- Nombres de endpoints: las rutas y nombres han sido normalizados a `register`, `create`, `by-email/{email}`, `{id}`, `{id}/active`.
- Creación por administrador: el servicio expone `CreateAdminUser` en la implementación concreta y el controlador lo llama desde `POST api/User/create`.
- Eliminación: ahora `DELETE` usa parámetro de ruta `{id}` en lugar de query string `?id=`.
- Respuestas: se mantiene el patrón simple de `Ok`/`BadRequest`/`NotFound`; el comportamiento de `403` queda delegado a las políticas/autorización.
