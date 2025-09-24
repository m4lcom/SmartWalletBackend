using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWallet.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; private set; }
        public Guid WalletId { get; private set; }
        public Wallet Wallet { get; private set; } = null!;

        [Required]
        public TransactionType Type { get; private set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }

        [Required]
        public TransactionStatus Status { get; private set; }
        public Guid? DestinationWalletId { get; private set; }
        public Wallet? DestinationWallet { get; private set; }
        public DateTime Timestamp { get; private set; }

        protected Transaction() { }

        public Transaction(
            Guid walletId,
            TransactionType type,
            decimal amount,
            Guid? destinationWalletId = null,
            TransactionStatus status = TransactionStatus.Pending)
        {
            TransactionId = Guid.NewGuid();
            WalletId = walletId;
            Type = type;
            Amount = amount;
            DestinationWalletId = destinationWalletId;
            Status = status;
            Timestamp = DateTime.UtcNow;
        }

        public void MarkAsPending()
        {
            if (Status != TransactionStatus.Pending)
            {
                throw new InvalidOperationException("Solo se puede marcar como Pending al crear la transaccion.");
            }
            Status = TransactionStatus.Pending;
        }

        public void MarkAsCompleted()
        {
            if (Status != TransactionStatus.Pending)
            {
                throw new InvalidOperationException("Solo una transaccion pendiente puede marcarse como Completed.");
            }
            Status = TransactionStatus.Completed;
        }

        public void MarkAsFailed()
        {
            if (Status == TransactionStatus.Completed)
            {
                throw new InvalidOperationException("No se puede marcar como Failed una transaccion ya completada.");
            }
            Status = TransactionStatus.Failed;
        }

        public void MarkAsCancelled()
        {
            if (Status == TransactionStatus.Completed)
            {
                throw new InvalidOperationException("No se puede marcar como Cancelled una transaccion ya completada.");
            }
            Status = TransactionStatus.Canceled;
        }
    }
}