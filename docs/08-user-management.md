# Gestión de Usuarios (User Management)

## Objetivo
Definir la arquitectura y responsabilidades de los componentes encargados de la gestión de usuarios en SmartWallet, reflejando la implementación actual de repositorios, servicios y controlador API.

---

## Componentes

### 1. Repositorio de Usuario (`IUserRepository` / `UserRepository`)
Responsable del acceso a datos de la entidad `User`. Desacopla la persistencia de la lógica de negocio y expone métodos asincrónicos usados por la capa de servicios.

**Responsabilidades:**
- Obtener todos los usuarios.
- Buscar usuario por ID o email.
- Crear, actualizar y (lógica de) eliminar usuarios.

**Firmas principales (implementación actual):**
- `Task<IEnumerable<User>> GetAllAsync()`
- `Task<User?> GetByIdAsync(Guid id)`
- `Task<User?> GetUserByEmailAsync(string email)`
- `Task CreateAsync(User user)`
- `Task UpdateAsync(User user)`
- (La eliminación física no se usa; se emplea desactivación lógica vía `UpdateAsync`)

**Implementación destacada:**
- `GetUserByEmailAsync` realiza la búsqueda por correo usando EF Core / LINQ en el contexto de persistencia.

**Ubicación:**  
`src/SmartWallet.Application/Abstractions/IUserRepository.cs`  
`src/SmartWallet.Infrastructure/Persistence/Repositories/UserRepository.cs`

---

### 2. Servicio de Usuario (`IUserServices` / `UserServices`)
Contiene la lógica de negocio y casos de uso relacionados con usuarios. Orquesta la interacción entre controladores y repositorios, mapea entidades a DTOs y aplica validaciones/reglas.

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

**Ubicación:**  
`src/SmartWallet.Application/Services/IUserServices.cs`  
`src/SmartWallet.Application/Services/UserServices.cs`

---

### 3. Controlador de Usuario (`UserController`)
Expone los endpoints HTTP. Todos los handlers delegan en `IUserServices` y retornan `IActionResult`. El controlador requiere autorización con rol `Admin`.

**Atributos clave:**
- `[Route("api/[controller]")]`
- `[ApiController]`
- `[Authorize(Roles = "Admin")]`

**Endpoints actuales (rutas tal y como están en el código):**
| Método | Ruta                                 | Descripción                                 | Parámetros / DTOs |
|--------|--------------------------------------|---------------------------------------------|-------------------|
| GET    | `/api/User`                          | Listar todos los usuarios                   | —                 |
| GET    | `/UserById/{userId}`                 | Obtener usuario por ID                      | route: `userId`   |
| GET    | `/UserByEmail/{email}`               | Obtener usuario por email                   | route: `email`    |
| POST   | `/RegisterUser`                      | Registro público (crea usuario Regular)     | body: `UserCreateRequest` |
| POST   | `/CreateUser`                        | Crear usuario (admin)                       | body: `UserCreateRequest` |
| PUT    | `/api/User/{id}`                     | Actualizar usuario                          | route: `id`, body: `UserUpdateDataRequest` |
| PUT    | `/ChangeActiveStatus/{id}`           | Alternar estado activo del usuario          | route: `id`       |
| DELETE | `/api/User` (DELETE con query `id`)  | Eliminar usuario (baja lógica, query param) | query: `id`       |

Notas:
- Las rutas que comienzan con `/` en los atributos del controlador (p. ej. `"/UserById/{userId}"`) son rutas absolutas y no incluyen el prefijo `api/[controller]`.
- Todas las acciones usan `IUserServices` y la mayoría devuelven códigos estándar: `200 OK`, `204 NoContent` (en diseños alternativos), `400 BadRequest` y `404 NotFound` según resultado.
- El controlador aplica `[Authorize(Roles = "Admin")]`: solo administradores pueden invocar estas rutas en el despliegue actual. Si algunas rutas deben ser públicas (p. ej. `RegisterUser`), ajustar el atributo `Authorize` o anularlo con `[AllowAnonymous]`.

**Ubicación:**  
`src/SmartWallet.API/Controllers/UserController.cs`

---

## Flujo de una operación típica

1. El cliente realiza una petición HTTP al `UserController`.
2. El controlador valida el modelo (model binding / DataAnnotations) y las credenciales (Authorize).
3. El controlador llama al método asincrónico del servicio (`IUserServices`).
4. El servicio aplica la lógica de negocio y usa `IUserRepository` (métodos `*Async`) para persistencia.
5. El servicio mapea entidades a `Contracts.Responses.UserResponse` y devuelve resultado al controlador.
6. El controlador construye la respuesta HTTP apropiada.

---

## Ejemplos de uso actualizados (firmas y rutas reales)

```csharp
// Obtener todos los usuarios
public ActionResult<List<UserResponse>> GetAllUsers()
{
    var users = _userServices.GetAllUsers();
    return Ok(users);
}

// Obtener un usuario por ID
public ActionResult<UserResponse> GetUserById(Guid userId)
{
    var user = _userServices.GetUserById(userId);
    if (user == null) return NotFound();

    return Ok(user);
}

// Obtener un usuario por email
public ActionResult<UserResponse> GetUserByEmail(string email)
{
    var user = _userServices.GetUserByEmail(email);
    if (user == null) return NotFound();

    return Ok(user);
}

// Registrar un nuevo usuario (público)
[HttpPost]
public IActionResult RegisterUser([FromBody] UserCreateRequest request)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    _userServices.RegisterUser(request);

    return CreatedAtAction(nameof(GetUserById), new { id = request.Id }, request);
}

// Crear un nuevo usuario (admin)
[HttpPost("CreateUser")]
public IActionResult CreateUser([FromBody] UserCreateRequest request)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    _userServices.CreateUser(request);

    return CreatedAtAction(nameof(GetUserById), new { id = request.Id }, request);
}

// Actualizar usuario existente
[HttpPut("{id}")]
public IActionResult UpdateUser(Guid id, [FromBody] UserUpdateDataRequest request)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var updated = _userServices.UpdateUser(id, request);
    if (!updated) return NotFound();

    return NoContent();
}

// Cambiar estado activo de usuario
[HttpPut("ChangeActiveStatus/{id}")]
public IActionResult ChangeUserActiveStatus(Guid id)
{
    var changed = _userServices.ChangeUserActiveStatus(id);
    if (!changed) return NotFound();

    return NoContent();
}

// Eliminar usuario (baja lógica)
[HttpDelete("{id}")]
public IActionResult DeleteUser(Guid id)
{
    var deleted = _userServices.DeleteUser(id);
    if (!deleted) return NotFound();

    return NoContent();
}
