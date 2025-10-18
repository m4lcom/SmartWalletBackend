using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWallet.Domain.Entities
{
    /// <summary>
    ///    representa una transaccion solicitada por un usuario (deposito, retiro, transferencia).
    ///    es mutable en su estado, porque refleja el ciclo de vida de la transaccion.
    /// </summary>
    public class Transaction
    {
        // --- identidad y metadatos ---
        [Key]
        public Guid Id { get; private set; }

        public TransactionType Type { get; private set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }
        
        public CurrencyCode CurrencyCode { get; private set; }

        public TransactionStatus Status { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        
        // --- relaciones con wallet origen ---
        public Guid WalletId { get; private set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; private set; } = null!;

        // --- relaciones con wallet destino para transferencias ---
        public Guid? DestinationWalletId { get; private set; }

        [ForeignKey("DestinationWalletId")]
        public Wallet? DestinationWallet { get; private set; }


        // --- constructores ---
        protected Transaction() { }

        public Transaction(
            Guid walletId, Guid? destinationWalletId, decimal amount, CurrencyCode currency, TransactionType type)
        {
                if (type == TransactionType.Transfer && destinationWalletId == null)
                    throw new ArgumentException("Las transacciones de tipo Transfer requieren DestinationWalletId.");

                if (type != TransactionType.Transfer && destinationWalletId != null)
                    throw new ArgumentException("Solo las transacciones de tipo Transfer deben tener DestinationWalletId.");

                if( amount <= 0)
                    throw new ArgumentException("El monto debe ser mayor a cero.", nameof(amount));

                Id = Guid.NewGuid();
                WalletId = walletId;
                DestinationWalletId = destinationWalletId;
                Amount = amount;
                CurrencyCode = currency;
                Type = type;
                Status = TransactionStatus.Pending;
                CreatedAt = DateTimeOffset.UtcNow;
        }


        // --- factory methods ---
        public static Transaction CreateDeposit(Guid walletId, decimal amount, CurrencyCode currency)
            => new Transaction(walletId, null, amount, currency, TransactionType.Deposit);

        public static Transaction CreateWithdrawal(Guid walletId, decimal amount, CurrencyCode currency)
            => new Transaction(walletId, null, amount, currency, TransactionType.Withdrawal);

        public static Transaction CreateTransfer(Guid sourceWalletId, Guid destinationWalletId, decimal amount, CurrencyCode currency)
            => new Transaction(sourceWalletId, destinationWalletId, amount, currency,  TransactionType.Transfer);
        
        // --- metodos de dominio ---
        public void MarkAsCompleted()
        {
            if (Status != TransactionStatus.Pending)
            {
                throw new InvalidOperationException("Solo una transaccion pendiente puede marcarse como Completada.");
            }
            Status = TransactionStatus.Completed;
        }

        public void MarkAsFailed()
        {
            if (Status == TransactionStatus.Completed)
            {
                throw new InvalidOperationException("No se puede marcar como Fallida una transaccion ya completada.");
            }
            Status = TransactionStatus.Failed;
        }

        public void MarkAsCanceled()
        {
            if (Status == TransactionStatus.Completed)
            {
                throw new InvalidOperationException("no se puede marcar como Cancelada una transaccion ya completada.");
            }
            Status = TransactionStatus.Canceled;
        }
    }
}