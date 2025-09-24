# domain-base-model

## Objetivo
Implementar el modelo base del dominio de **SmartWallet**, definiendo las entidades principales y sus validaciones, manteniendo el dominio libre de dependencias de infraestructura.

---

## Pasos de implementación
1. Creación de las entidades `User`, `Wallet` y `Transaction` en `SmartWallet.Domain.Entities`.
2. Incorporación de validaciones con **DataAnnotations** en propiedades clave.
3. Definición de relaciones:
   - `User` → relación 1:1 con `Wallet`.
   - `Wallet` → relación 1:N con `Transaction`.
4. Propiedades de navegación marcadas como anulables para evitar advertencias **CS8618**.
5. Eliminación de cualquier referencia a **EF Core** en el dominio para mantenerlo puro.

---

## Endpoints
No aplica en esta etapa.  
La feature define únicamente el modelo de dominio y no expone endpoints.

---

## Cambios en base de datos
- Tablas previstas (a generar en la capa de infraestructura):
  - `Users`
  - `Wallets`
  - `Transactions`
- Sin migraciones creadas aún.

---

## Ejemplos de uso
```
var user = new User("Juan Pérez", "juan@example.com", hashService.Hash("1234"), UserRole.Standard);

var wallet = new Wallet(
    userId: user.UserID,
    name: "Ahorros",
    currencyCode: "ARS",
    alias: "ahorros.principal",
    initialBalance: 0m
);

var tx = new Transaction(
    walletId: wallet.WalletID,
    type: TransactionType.Credit,
    amount: 1500.00m
);
```