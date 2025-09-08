using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWallet.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; private set; }
        [Required]
        public Guid WalletId { get; private set; }
        public Wallet? Wallet { get; private set; }
        [Required]
        public TransactionType Type { get; private set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }

        // wallet contraparte en transacciones entre dos wallets
        public Guid? ReferenceWalletId { get; private set; }
        public Wallet? ReferenceWallet { get; private set; }
        [Required]
        public DateTime Timestamp { get; private set; }

        protected Transaction() { }

        public Transaction(
            Guid walletId,
            TransactionType type,
            decimal amount,
            Guid? referenceWalletId = null)
        {
            TransactionId = Guid.NewGuid();
            WalletId = walletId;
            Type = type;
            Amount = amount;
            ReferenceWalletId = referenceWalletId;
            Timestamp = DateTime.UtcNow;
        }
    }
}