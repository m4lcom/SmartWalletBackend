using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWallet.Domain.Entities
{
    /// Cada TransactionLedger refleja el impacto de una transacción en una wallet.
    public class TransactionLedger
    {
        // --- identidad y metadatos ---
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public DateTimeOffset Timestamp { get; private set; }

        [Required]
        public TransactionType Type { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Amount { get; private set; }

        [Required]
        public CurrencyCode CurrencyCode { get; private set; }

        [Required]
        public TransactionStatus Status { get; private set; }

        public string? Metadata { get; private set; }

        // --- relaciones con transacciones y wallets ---
        public Guid? SourceTransactionId { get; private set; }
        public Guid? SourceWalletId { get; private set; }

        [ForeignKey("SourceWalletId")]
        public Wallet? SourceWallet { get; private set; }
        public Guid? DestinationTransactionId { get; private set; }
        public Guid? DestinationWalletId { get; private set; }

        [ForeignKey("DestinationWalletId")]
        public Wallet? DestinationWallet { get; private set; }

        // --- constructores ---
        private TransactionLedger() { }

        private TransactionLedger(
            TransactionType type,
            decimal amount,
            CurrencyCode currencyCode,
            TransactionStatus status,
            Guid? sourceWalletId = null,
            Guid? destinationWalletId = null,
            Guid? sourceTransactionId = null,
            Guid? destinationTransactionId = null,
            string? metadata = null)
        {
            if (amount <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.", nameof(amount));

            Id = Guid.NewGuid();
            Timestamp = DateTimeOffset.UtcNow;
            Type = type;
            Amount = amount;
            CurrencyCode = currencyCode;
            Status = status;
            SourceWalletId = sourceWalletId;
            DestinationWalletId = destinationWalletId;
            SourceTransactionId = sourceTransactionId;
            DestinationTransactionId = destinationTransactionId;
            Metadata = metadata;
        }

        // --- factory methods ---
        public static TransactionLedger CreateDeposit(
            Guid walletId,
            Guid transactionId,
            decimal amount,
            CurrencyCode currency,
            string? metadata = null)
            => new TransactionLedger(
                TransactionType.Deposit,
                amount,
                currency,
                TransactionStatus.Pending,
                null,
                walletId,
                null,
                transactionId,
                metadata);

        public static TransactionLedger CreateWithdrawal(
            Guid walletId,
            Guid transactionId,
            decimal amount,
            CurrencyCode currency,
            string? metadata = null)
            => new TransactionLedger(
                TransactionType.Withdrawal,
                amount,
                currency,
                TransactionStatus.Pending,
                walletId,
                null,
                transactionId,
                null,
                metadata);

        public static TransactionLedger CreateTransfer(
            Guid sourceWalletId,
            Guid destinationWalletId,
            Guid? sourceTransactionId,
            Guid? destinationTransactionId,
            decimal amount,
            CurrencyCode currency,
            string? metadata = null)
            => new TransactionLedger(
                TransactionType.Transfer,
                amount,
                currency,
                TransactionStatus.Pending,
                sourceWalletId,
                destinationWalletId,
                sourceTransactionId,
                destinationTransactionId,
                metadata);

        // --- conversion desde transaction ---
        public static List<TransactionLedger> FromTransaction(Transaction transaction, string? metadata = null)
        {
            return transaction.Type switch
            {
                TransactionType.Deposit => new List<TransactionLedger>
        {
            CreateDeposit(transaction.WalletId, transaction.Id, transaction.Amount, transaction.CurrencyCode, metadata)
        },

                TransactionType.Withdrawal => new List<TransactionLedger>
        {
            CreateWithdrawal(transaction.WalletId, transaction.Id, transaction.Amount, transaction.CurrencyCode, metadata)
        },

                TransactionType.Transfer => new List<TransactionLedger>
        {
            // asiento de débito (wallet origen)
            CreateTransfer(
                transaction.WalletId,
                transaction.DestinationWalletId!.Value,
                transaction.Id,
                null,
                transaction.Amount,
                transaction.CurrencyCode,
                metadata
            ),

            // asiento de crédito (wallet destino)
            CreateTransfer(
                transaction.DestinationWalletId!.Value,
                transaction.WalletId,
                null,
                transaction.Id,
                transaction.Amount,
                transaction.CurrencyCode,
                metadata
            )
        },

                _ => throw new InvalidOperationException("Tipo de transacción no soportado.")
            };
        }




        // --- metodos de dominio para estados ---
        public void MarkAsPending()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Solo se puede marcar como Pending al crear la transacción.");
            Status = TransactionStatus.Pending;
        }

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
    }
}
