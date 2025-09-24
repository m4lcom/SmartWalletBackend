# Transaction Lifecycle

## Objetivo

Definir el ciclo de vida de las transacciones en **SmartWallet**, asegurando consistencia, auditabilidad y trazabilidad entre operaciones atómicas (`Transaction`) y operaciones de negocio (`TransactionLedger`).

---

## Entidades involucradas

### Transaction

- Movimiento atómico sobre una wallet.
- Tipos: `Deposit`, `Withdrawal`.
- Cada transacción afecta una sola wallet.

### TransactionLedger

- Registro de la operación de negocio.
- Tipos: `Deposit`, `Withdrawal`, `Transfer`.
- En `Transfer`, vincula dos transacciones atómicas mediante `SourceTransactionId` y `DestinationTransactionId`.

---

## Estados (`TransactionStatus`)

- `Pending`: transacción creada, aún no ejecutada.
- `Completed`: ejecutada con éxito.
- `Failed`: intentada pero fallida.
- `Canceled`: abortada antes de completarse.

---

## Reglas de transición

- Solo una transacción en estado `Pending` puede pasar a `Completed`, `Failed` o `Canceled`.
- Una vez en estado `Completed`, no puede modificarse.
- `Failed` y `Canceled` son estados finales.

---

## Métodos de dominio

```
public void MarkAsCompleted()
{
    if (Status != TransactionStatus.Pending)
        throw new InvalidOperationException("Solo una transacción pendiente puede marcarse como Completed.");
    Status = TransactionStatus.Completed;
}

public void MarkAsFailed()
{
    if (Status == TransactionStatus.Completed)
        throw new InvalidOperationException("No se puede marcar como Failed una transacción ya completada.");
    Status = TransactionStatus.Failed;
}

public void MarkAsCanceled()
{
    if (Status == TransactionStatus.Completed)
        throw new InvalidOperationException("No se puede marcar como Canceled una transacción ya completada.");
    Status = TransactionStatus.Canceled;
}
