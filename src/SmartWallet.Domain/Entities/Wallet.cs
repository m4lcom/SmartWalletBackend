
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SmartWallet.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public Guid WalletID { get; private set; }
        [Required]
        public Guid UserID { get; private set; }
        public User User { get; private set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; private set; }
        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Z]{3}$")]
        public string CurrencyCode { get; private set; }
        [Required]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "El alias debe tener entre 9 y 20 caracteres.")]
        [RegularExpression(@"^[A-Za-z\.]+$", ErrorMessage = "El alias solo puede contener letras y puntos.")]
        public string Alias { get; private set; }
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; private set; }
        [Required]
        public DateTime CreatedAt { get; private set; }

        private readonly List<Transaction> _transactions = new();
        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

        protected Wallet() { }

        public Wallet(Guid userId, string name, string currencyCode = "ARS", string alias, decimal initialBalance = 0)
        {
            WalletID = Guid.NewGuid();
            UserID = userId;
            Name = name;
            CurrencyCode = currencyCode.ToUpper(CultureInfo.InvariantCulture);
            Alias = alias.ToLower(CultureInfo.InvariantCulture);
            Balance = initialBalance;
            CreatedAt = DateTime.UtcNow;
        }


    }
}
