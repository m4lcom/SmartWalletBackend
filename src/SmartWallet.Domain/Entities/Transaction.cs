using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWallet.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; private set; }

        // --- relaciones con wallet origen ---
        public Guid WalletId { get; private set; }

        [ForeignKey("WalletId")]
        public Wallet Wallet { get; private set; } = null!;

        // --- relaciones con wallet destino para transferencias ---
        public Guid? DestinationWalletId { get; private set; }

        [ForeignKey("DestinationWalletId")]
        public Wallet? DestinationWallet { get; private set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }

        [Required]
        public CurrencyCode CurrencyCode { get; private set; }

        [Required]
        public TransactionType Type { get; private set; }

        [Required]
        public TransactionStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // --- constructores ---
        protected Transaction() { }

        public Transaction(
            Guid walletId,
            TransactionType type,
            decimal amount,
            CurrencyCode currencyCode,
            Guid? destinationWalletId = null,
            TransactionStatus status = TransactionStatus.Pending)
        {
            if (type == TransactionType.Transfer && destinationWalletId == null)
                throw new ArgumentException("las transacciones de tipo transferencia requieren DestinationWalletId.");

            if (type != TransactionType.Transfer && destinationWalletId != null)
                throw new ArgumentException("solo las transacciones de tipo transferencia deben tener DestinationWalletId.");

            if( amount <= 0)
                throw new ArgumentException("el monto debe ser mayor a cero.", nameof(amount));

            Id = Guid.NewGuid();
            WalletId = walletId;
            Type = type;
            Amount = amount;
            CurrencyCode = currencyCode;
            DestinationWalletId = destinationWalletId;
            Status = status;
            CreatedAt = DateTime.UtcNow;
        }

        // --- metodos de dominio ---
        public void MarkAsPending()
        {
            if (Status != TransactionStatus.Pending)
            {
                throw new InvalidOperationException("solo se puede marcar como Pending al crear la transaccion.");
            }
            Status = TransactionStatus.Pending;
        }

        public void MarkAsCompleted()
        {
            if (Status != TransactionStatus.Pending)
            {
                throw new InvalidOperationException("solo una transaccion pendiente puede marcarse como Completed.");
            }
            Status = TransactionStatus.Completed;
        }

        public void MarkAsFailed()
        {
            if (Status == TransactionStatus.Completed)
            {
                throw new InvalidOperationException("no se puede marcar como Failed una transaccion ya completada.");
            }
            Status = TransactionStatus.Failed;
        }

        public void MarkAsCancelled()
        {
            if (Status == TransactionStatus.Completed)
            {
                throw new InvalidOperationException("no se puede marcar como Cancelled una transaccion ya completada.");
            }
            Status = TransactionStatus.Canceled;
        }

        // --- factory methods ---
        public static Transaction CreateDeposit(Guid walletId, decimal amount, CurrencyCode currency)
    => new Transaction(walletId, TransactionType.Deposit, amount, currency);

        public static Transaction CreateWithdrawal(Guid walletId, decimal amount, CurrencyCode currency)
            => new Transaction(walletId, TransactionType.Withdrawal, amount, currency);

        public static Transaction CreateTransfer(Guid sourceWalletId, Guid destinationWalletId, decimal amount, CurrencyCode currency)
            => new Transaction(sourceWalletId, TransactionType.Transfer, amount, currency, destinationWalletId);

    }
}