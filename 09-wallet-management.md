# Gestión de Billeteras (Wallet Management)

## Objetivo
Definir la arquitectura y responsabilidades de los componentes encargados de la gestión de billeteras (`Wallet`) en SmartWallet, incluyendo repositorios, servicios de aplicación y controladores API.

---

## Componentes

### 1. Repositorio de Wallet (`IWalletRepository` / `WalletRepository`)
Responsable del acceso a datos de la entidad `Wallet`.  
Permite desacoplar la lógica de persistencia de la lógica de negocio.

**Responsabilidades:**
- Obtener una billetera por ID o alias.
- Verificar existencia de una billetera.
- Crear o actualizar billeteras.

**Métodos principales:**
- `Task<Wallet?> GetByIdAsync(Guid id)`
- `Task<bool> ExistsAsync(Guid id)`
- `Task AddAsync(Wallet wallet)`
- `Task UpdateAsync(Wallet wallet)`
- `Task<Wallet?> GetByAliasAsync(string alias)`

**Ubicación:**  
`src/SmartWallet.Application/Abstraction/IWalletRepository.cs`  
`src/SmartWallet.Infrastructure/Persistence/Repositories/WalletRepository.cs`

---

### 2. Servicio de Wallet (`IWalletService` / `WalletService`)
Contiene la lógica de negocio y casos de uso relacionados con billeteras.  
Orquesta la interacción entre los controladores y los repositorios.

**Responsabilidades:**
- Exponer operaciones de alto nivel para la gestión de wallets.
- Validar reglas de negocio antes de acceder al repositorio.
- Generar transacciones internas para depósitos, retiros y transferencias.

**Métodos principales:**
- `Task<Wallet?> GetByIdAsync(Guid id)`
- `Task<bool> ExistsAsync(Guid id)`
- `Task AddAsync(Wallet wallet)`
- `Task UpdateAsync(Wallet wallet)`
- `Task<Wallet> CreateAsync(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance)`

**Ubicación:**  
`src/SmartWallet.Application/Services/IWalletService.cs`  
`src/SmartWallet.Application/Services/WalletService.cs`

---

### 3. Controlador de Wallet (`WalletController`)
Expone los endpoints HTTP para la gestión de billeteras.  
Recibe y valida las solicitudes, delega la lógica al servicio y retorna respuestas adecuadas.

**Responsabilidades:**
- Definir rutas y métodos HTTP para operaciones CRUD de Wallet.
- Validar datos de entrada (model binding, DataAnnotations).
- Retornar respuestas HTTP estándar (200, 201, 400, 404, etc).

**Endpoints actuales (propuestos):**
| Método | Ruta                        | Descripción                           | Parámetros         |
|--------|-----------------------------|---------------------------------------|------------------|
| GET    | /api/wallets/{id}           | Obtener wallet por ID                  | id (path)        |
| GET    | /api/wallets/alias/{alias}  | Obtener wallet por alias               | alias (path)     |
| POST   | /api/wallets                | Crear nueva wallet                     | body (WalletCreateRequest) |
| PUT    | /api/wallets/{id}           | Actualizar wallet existente            | id (path), body  |
| POST   | /api/wallets/{id}/deposit   | Depositar dinero en wallet             | id (path), body  |
| POST   | /api/wallets/{id}/withdraw  | Retirar dinero de wallet               | id (path), body  |
| POST   | /api/wallets/{id}/transfer  | Transferir dinero a otra wallet        | id (path), body  |

**Ubicación:**  
`src/SmartWallet.API/Controllers/WalletController.cs`

---

## Flujo de una operación típica

1. **Controller** recibe la solicitud HTTP y valida el modelo.
2. Llama al **servicio** correspondiente (`WalletService`).
3. El servicio ejecuta la lógica de negocio:
   - Depositar, retirar o transferir dinero
   - Validar saldo suficiente para retiros y transferencias
4. Se utiliza el **repositorio** para persistir cambios en la base de datos.
5. El resultado se retorna al controller y finalmente al cliente con código HTTP adecuado.

---

## Ejemplo de uso actualizado

```csharp
// Crear una nueva wallet
[HttpPost]
public async Task<IActionResult> CreateWallet([FromBody] WalletCreateRequest request)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var wallet = await _walletService.CreateAsync(
        request.UserId,
        request.Name,
        request.CurrencyCode,
        request.Alias,
        request.InitialBalance
    );

    return CreatedAtAction(nameof(GetWalletById), new { id = wallet.Id }, wallet);
}

// Obtener wallet por ID
[HttpGet("{id}")]
public async Task<IActionResult> GetWalletById(Guid id)
{
    var wallet = await _walletService.GetByIdAsync(id);
    if (wallet == null) return NotFound();

    return Ok(wallet);
}

// Depositar dinero
[HttpPost("{id}/deposit")]
public async Task<IActionResult> Deposit(Guid id, [FromBody] WalletTransactionRequest request)
{
    var wallet = await _walletService.GetByIdAsync(id);
    if (wallet == null) return NotFound();

    var transaction = wallet.Deposit(request.Amount, request.CurrencyCode);
    await _walletRepository.UpdateAsync(wallet);

    return Ok(transaction);
}

// Retirar dinero
[HttpPost("{id}/withdraw")]
public async Task<IActionResult> Withdraw(Guid id, [FromBody] WalletTransactionRequest request)
{
    var wallet = await _walletService.GetByIdAsync(id);
    if (wallet == null) return NotFound();

    var transaction = wallet.Withdrawal(request.Amount, request.CurrencyCode);
    await _walletRepository.UpdateAsync(wallet);

    return Ok(transaction);
}

// Transferir dinero a otra wallet
[HttpPost("{id}/transfer")]
public async Task<IActionResult> Transfer(Guid id, [FromBody] WalletTransferRequest request)
{
    var wallet = await _walletService.GetByIdAsync(id);
    if (wallet == null) return NotFound();

    var transaction = wallet.Transfer(request.DestinationWalletId, request.Amount, request.CurrencyCode);
    await _walletRepository.UpdateAsync(wallet);

    return Ok(transaction);
}
