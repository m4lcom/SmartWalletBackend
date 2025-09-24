
using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SmartWallet.Domain.Entities;

public class Wallet
{
    [Key]
    public Guid WalletID { get; private set; }

    public Guid UserID { get; private set; }
    public User User { get; private set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; private set; } = string.Empty;
    public CurrencyCode CurrencyCode { get; private set; }
    
    [Required]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El alias debe tener entre 6 y 20 caracteres.")]
    [RegularExpression(@"^[A-Za-z\.]+$", ErrorMessage = "El alias solo puede contener letras y puntos.")]
    public string Alias { get; private set; } = string.Empty;

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    protected Wallet() { }

    public Wallet(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance = 0)
    {
        WalletID = Guid.NewGuid();
        UserID = userId;
        Name = name;
        CurrencyCode = currencyCode;
        Alias = alias.ToLower(CultureInfo.InvariantCulture);
        Balance = initialBalance;
        CreatedAt = DateTime.UtcNow;
    }

    // -- metodos de dominio -- 

    public void Debit(decimal amount) 
    {
        if (amount > 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");
        if (Balance < amount) throw new InvalidOperationException("Fondos insuficientes");
        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");
        Balance += amount;
    }

    public Transaction CreatedTransaction(
        TransactionType type,
        decimal amount,
        Guid? destinationWalletId = null,
        TransactionStatus status = TransactionStatus.Pending)
    {
        var transaction = new Transaction(WalletID, type, amount, destinationWalletId, status);
        _transactions.Add(transaction);
        return transaction;
    }


}
