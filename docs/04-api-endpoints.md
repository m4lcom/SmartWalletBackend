# API y Endpoints

# Endpoints – Transaction + TransactionLedger

## Objetivo
Documentar los endpoints expuestos por la vertical de transacciones financieras, incluyendo operaciones de depósito, extracción, transferencia y lectura de transacciones y ledgers asociados.

---

## Pasos de implementación
1. Se definieron los endpoints en `TransactionsController`.
2. Se conectaron con `TransactionService` para ejecutar la lógica de negocio.
3. Se agregaron rutas de lectura para `Transaction` y `TransactionLedger`.
4. Se validaron los endpoints vía Postman.

---

## Endpoints
| Método | Ruta                                                       | Descripción                                 | Autenticación |
|--------|------------------------------------------------------------|---------------------------------------------|---------------|
| POST   | /api/transactions/deposit                                  | Realiza un depósito en una wallet           | ✅ |
| POST   | /api/transactions/withdraw                                 | Realiza una extracción                      | ✅ |
| POST   | /api/transactions/transfer                                 | Transfiere entre dos wallets                | ✅ |
| GET    | /api/transactions/{id}                                     | Obtiene una transacción por ID              | ✅ |
| GET    | /api/transactions-ledger/{id}                              | Obtiene un ledger por ID                    | ✅ |
| GET    | /api/transactions-ledger/by-transaction/{transactionId}    | Obtiene el ledger vinculado a una transacción | ✅ |

---

## Cambios en base de datos
- Sin cambios estructurales en esta etapa.
- Se utilizaron las tablas existentes `Transactions` y `TransactionLedgers`.

---

## Ejemplos de uso
```bash
curl -X POST https://localhost:7281/api/transactions/transfer \
-H "Content-Type: application/json" \
-H "Authorization: Bearer {token}" \
-d '{
  "sourceWalletId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "destinationWalletId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "amount": 250,
  "currencyCode": 32
}'
