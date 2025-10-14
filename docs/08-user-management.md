# Gestión de Usuarios (User Management)

## Objetivo
Definir la arquitectura y responsabilidades de los componentes encargados de la gestión de usuarios en SmartWallet, abarcando repositorios, servicios de aplicación y controladores API.

---

## Componentes

### 1. Repositorio de Usuario (`IUserRepository` / `UserRepository`)
Responsable del acceso a datos de la entidad `User`.  
Permite desacoplar la lógica de persistencia de la lógica de negocio.

**Responsabilidades:**
- Obtener todos los usuarios.
- Buscar usuario por ID o email.
- Crear, actualizar y eliminar usuarios.

**Métodos principales:**
- `IEnumerable<User> GetAll()`
- `User? GetById(Guid id)`
- `User? GetUserByEmail(string email)`
- `void Create(User user)`
- `void Update(User user)`
- `void Delete(User user)`

**Implementación destacada:**
- El método `GetUserByEmail` permite buscar eficientemente un usuario por su correo electrónico usando LINQ sobre el contexto de EF Core.

**Ubicación:**  
`src/SmartWallet.Application/Abstraction/IUserRepository.cs`  
`src/SmartWallet.Infrastructure/Persistence/Repositories/UserRepository.cs`

---

### 2. Servicio de Usuario (`IUserServices` / `UserServices`)
Contiene la lógica de negocio y casos de uso relacionados con usuarios.  
Orquesta la interacción entre los controladores y los repositorios.

**Responsabilidades:**
- Exponer operaciones de alto nivel para la gestión de usuarios.
- Validar reglas de negocio antes de acceder al repositorio.
- Mapear entidades de dominio a DTOs de respuesta.

**Métodos principales:**
- `List<UserResponse> GetAllUsers()`
- `UserResponse? GetUserById(Guid id)`
- `UserResponse? GetUserByEmail(string email)`
- `bool CreateUser(UserCreateRequest request)`
- `bool UpdateUser(Guid id, UserUpdateDataRequest request)`
- `bool ChangeUserActiveStatus(Guid id)`
- `bool DeleteUser(Guid id)`

**Cambios recientes:**
- Ahora las operaciones de actualización, cambio de estado activo y eliminación usan el `Guid id` del usuario en vez del email.
- Se agregó el método `ChangeUserActiveStatus(Guid id)` para alternar el estado activo/inactivo del usuario.
- La eliminación lógica (`DeleteUser`) ahora solo desactiva el usuario (no lo borra físicamente).

**Ubicación:**  
`src/SmartWallet.Application/Services/IUserServices.cs`  
`src/SmartWallet.Application/Services/UserServices.cs`

---

### 3. Controlador de Usuario (`UserController`)
Expone los endpoints HTTP para la gestión de usuarios.  
Recibe y valida las solicitudes, delega la lógica al servicio y retorna respuestas adecuadas.

**Responsabilidades:**
- Definir rutas y métodos HTTP para operaciones CRUD de usuario.
- Validar datos de entrada (model binding, DataAnnotations).
- Retornar respuestas HTTP estándar (200, 201, 400, 404, etc).

**Endpoints actuales:**
| Método | Ruta                        | Descripción                        | Parámetros         |
|--------|-----------------------------|------------------------------------|--------------------|
| GET    | /api/usercontroller         | Listar todos los usuarios          |                    |
| GET    | /UserById/                  | Obtener usuario por ID             | userId (query)     |
| GET    | /api/usercontroller         | Obtener usuario por email          | email (query)      |
| POST   | /api/usercontroller         | Crear nuevo usuario                | body (UserCreate)  |
| PUT    | /api/usercontroller         | Actualizar usuario                 | id (query), body   |
| PUT    | /ChangeActiveStatus/        | Cambiar estado activo del usuario  | id (query)         |
| DELETE | /api/usercontroller         | Eliminar usuario (lógico)          | id (query)         |

**Notas:**
- Los endpoints de actualización, eliminación y cambio de estado ahora usan el identificador `Guid id` como parámetro de consulta.
- El endpoint de eliminación realiza una baja lógica (desactiva el usuario).

**Ubicación:**  
`src/SmartWallet.API/Controllers/UserController.cs`

---

## Flujo de una operación típica

1. **Controller** recibe la solicitud HTTP y valida el modelo.
2. Llama al **servicio** correspondiente (`UserServices`).
3. El servicio ejecuta la lógica de negocio y utiliza el **repositorio** para acceder a los datos.
4. El resultado se mapea a un DTO de respuesta y se retorna al controller.
5. El controller responde al cliente con el resultado y el código HTTP adecuado.

---

## Ejemplo de uso actualizado

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

// Crear un nuevo usuario
[HttpPost]
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

