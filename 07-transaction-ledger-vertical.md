# Implementación vertical de Transaction + TransactionLedger

## Objetivo
Completar la vertical de transacciones financieras en SmartWallet, implementando los DTOs, servicios de aplicación, controladores y endpoints para operar sobre las entidades `Transaction` y `TransactionLedger`.  
Las entidades ya estaban definidas previamente en la capa Domain.

---

## Pasos de implementación
1. Se implementaron los DTOs: `DepositRequest`, `WithdrawRequest`, `TransferRequest`.
2. Se creó el servicio `TransactionService` con métodos `DepositAsync`, `WithdrawAsync`, `TransferAsync`.
3. Se definió la interfaz `ITransactionService` y se registró en `Program.cs`.
4. Se implementaron los repositorios `TransactionRepository` y `TransactionLedgerRepository`.
5. Se registraron ambos repositorios en el contenedor de dependencias.
6. Se definieron los endpoints en `TransactionsController`.
7. Se agregaron endpoints de lectura para `Transaction` y `TransactionLedger`.
8. Se testearon los endpoints vía Postman con valores válidos (`CurrencyCode = 32`).
9. Se resolvieron errores 405 y 400 relacionados con registro de servicios y validación de enums.
10. Se documentó la verticalidad completa para onboarding y mantenimiento.

---

## Endpoints
| Método | Ruta                                 | Descripción                              | Autenticación |
|--------|--------------------------------------|------------------------------------------|---------------|
| POST   | /api/transactions/deposit            | Realiza un depósito en una wallet        | ✅ |
| POST   | /api/transactions/withdraw           | Realiza una extracción                   | ✅ |
| POST   | /api/transactions/transfer           | Transfiere entre dos wallets             | ✅ |
| GET    | /api/transactions/{id}               | Obtiene una transacción por ID           | ✅ |
| GET    | /api/transactions-ledger/{id}        | Obtiene un ledger por ID                 | ✅ |
| GET    | /api/transactions-ledger/by-transaction/{transactionId} | Ledger vinculado a una transacción | ✅ |

---

## Cambios en base de datos
- Sin cambios en esta etapa: las entidades `Transaction` y `TransactionLedger` ya estaban creadas previamente.
- Se utilizaron las tablas existentes para persistencia y testeo.

---

## Ejemplos de uso
```bash
curl -X POST https://localhost:7281/api/transactions/deposit \
-H "Content-Type: application/json" \
-d '{
  "walletId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "amount": 500,
  "currencyCode": 32
}'
