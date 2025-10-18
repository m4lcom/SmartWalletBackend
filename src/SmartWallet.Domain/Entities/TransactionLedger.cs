using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWallet.Domain.Entities
{
    /// <summary>
    /// cada TransactionLedger refleja el impacto de una transacción en una wallet.
    /// es inmutable: nunca se modifica, siempre se corrige con nuevos asientos.
    /// </summary>
    public class TransactionLedger
    {
        // --- identidad y metadatos ---
        [Key]
        public Guid Id { get; private set; }

        public TransactionType Type { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Amount { get; private set; }

        public CurrencyCode CurrencyCode { get; private set; }

        public LedgerStatus Status { get; private set; }
        
        public DateTimeOffset Timestamp { get; private set; }
        public string? Metadata { get; private set; }

        // --- relaciones con transacciones y wallets ---
        public Guid TransactionId { get; private set; }
        public Guid WalletId { get; private set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; private set; } = null!;

        // --- constructores ---
        private TransactionLedger() { }

        private TransactionLedger(
            TransactionType type,
            decimal amount,
            CurrencyCode currencyCode,
            LedgerStatus status,
            Guid transactionId,
            Guid walletId,
            string? metadata = null)
        {
            if (amount == 0)
                throw new ArgumentException("El monto no puede ser cero.", nameof(amount));

            Id = Guid.NewGuid();
            Timestamp = DateTimeOffset.UtcNow;
            Type = type;
            Amount = amount;
            CurrencyCode = currencyCode;
            Status = status;
            TransactionId = transactionId;
            WalletId = walletId;
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
                LedgerStatus.Completed,
                transactionId,
                walletId,
                metadata ?? "Deposit");

        public static TransactionLedger CreateWithdrawal(
            Guid walletId,
            Guid transactionId,
            decimal amount,
            CurrencyCode currency,
            string? metadata = null)
            => new TransactionLedger(
                TransactionType.Withdrawal,
                - amount,
                currency,
                LedgerStatus.Completed,
                transactionId,
                walletId,
                metadata ?? "Withdrawal");

        public static List<TransactionLedger> CreateTransfer(
            Guid sourceWalletId,
            Guid destinationWalletId,
            Guid transactionId,
            decimal amount,
            CurrencyCode currency,
            string? metadata = null)
            => new List<TransactionLedger>
            {
                new TransactionLedger(TransactionType.Transfer, -amount, currency, LedgerStatus.Completed, transactionId, sourceWalletId, metadata ?? "Transfer Debit"),
                new TransactionLedger(TransactionType.Transfer, amount, currency, LedgerStatus.Completed, transactionId, destinationWalletId, metadata ?? "Transfer Credit")
            };

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

                TransactionType.Transfer => CreateTransfer(transaction.WalletId, transaction.DestinationWalletId!.Value, transaction.Id, transaction.Amount, transaction.CurrencyCode, metadata),
                _ => throw new InvalidOperationException("Tipo de transacción no soportado.")
            };
        }

        // --- metodos de correcion ---
        public TransactionLedger CreateReversal(string? metadata = null)
        {
            return new TransactionLedger(this.Type, -this.Amount, this.CurrencyCode, LedgerStatus.Reversed, this.TransactionId, this.WalletId, metadata ?? $"Reversal of {this.Id}");
        }

        public TransactionLedger CreateAdjustment(decimal newAmount, string? metadata = null)
        {
            var difference = newAmount - this.Amount;
            if (difference == 0)
                throw new InvalidOperationException("No se puede crear un ajuste sin diferencia de monto");
            return new TransactionLedger(this.Type, difference, this.CurrencyCode, LedgerStatus.Adjustment, this.TransactionId, this.WalletId, metadata ?? $"Adjustment of {this.Id}");
        }
    }
}
