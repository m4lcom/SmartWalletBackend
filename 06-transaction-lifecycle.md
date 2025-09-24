# Transaction Lifecycle

## Objetivo

Definir el ciclo de vida de las transacciones en **SmartWallet**, asegurando consistencia, auditabilidad y trazabilidad entre operaciones at�micas (`Transaction`) y operaciones de negocio (`TransactionLedger`).

---

## Entidades involucradas

### Transaction

- Movimiento at�mico sobre una wallet.
- Tipos: `Deposit`, `Withdrawal`.
- Cada transacci�n afecta una sola wallet.

### TransactionLedger

- Registro de la operaci�n de negocio.
- Tipos: `Deposit`, `Withdrawal`, `Transfer`.
- En `Transfer`, vincula dos transacciones at�micas mediante `SourceTransactionId` y `DestinationTransactionId`.

---

## Estados (`TransactionStatus`)

- `Pending`: transacci�n creada, a�n no ejecutada.
- `Completed`: ejecutada con �xito.
- `Failed`: intentada pero fallida.
- `Canceled`: abortada antes de completarse.

---

## Reglas de transici�n

- Solo una transacci�n en estado `Pending` puede pasar a `Completed`, `Failed` o `Canceled`.
- Una vez en estado `Completed`, no puede modificarse.
- `Failed` y `Canceled` son estados finales.

---

## M�todos de dominio

```
public void MarkAsCompleted()
{
    if (Status != TransactionStatus.Pending)
        throw new InvalidOperationException("Solo una transacci�n pendiente puede marcarse como Completed.");
    Status = TransactionStatus.Completed;
}

public void MarkAsFailed()
{
    if (Status == TransactionStatus.Completed)
        throw new InvalidOperationException("No se puede marcar como Failed una transacci�n ya completada.");
    Status = TransactionStatus.Failed;
}

public void MarkAsCanceled()
{
    if (Status == TransactionStatus.Completed)
        throw new InvalidOperationException("No se puede marcar como Canceled una transacci�n ya completada.");
    Status = TransactionStatus.Canceled;
}
