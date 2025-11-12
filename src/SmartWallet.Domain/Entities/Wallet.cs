
using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SmartWallet.Domain.Entities;

public class Wallet
{
    // --- identidad y metadatos ---
    [Key]
    public Guid Id { get; private set; }

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


    // Navegaciones inversas
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>(); // origen
    public ICollection<Transaction> ReceivedTransfers { get; private set; } = new List<Transaction>(); // destino
    public ICollection<TransactionLedger> TransactionLedgers { get; private set; } = new List<TransactionLedger>(); // asientos


    // --- constructores ---
    protected Wallet() { }

    public Wallet(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance = 0)
    {
        Id = Guid.NewGuid();
        UserID = userId;
        Name = name;
        CurrencyCode = currencyCode;
        Alias = alias.ToLower(CultureInfo.InvariantCulture);
        Balance = initialBalance;
        CreatedAt = DateTime.UtcNow;
    }


    // -- metodos internos de invariantes -- 
    private void Debit(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");
        if (Balance < amount) throw new InvalidOperationException("Fondos insuficientes.");
        Balance -= amount;
    }

    private void Credit(decimal amount)
    {
        if (amount <= 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");
        Balance += amount;
    }


    // --- metodos de dominio ---
    public Transaction Deposit(decimal amount, CurrencyCode currency)
    {
        Credit(amount);
        return Transaction.CreateDeposit(Id, amount, currency);
    }

    public Transaction Withdrawal(decimal amount,  CurrencyCode currency)
    {
        Debit(amount);
        return Transaction.CreateWithdrawal(Id, amount, currency);
    }
    
    public Transaction Transfer(Wallet destinationWallet, decimal amount, CurrencyCode currency)
    {
        Debit(amount);
        destinationWallet.ApplyCredit(amount, currency);
        return Transaction.CreateTransfer(Id, destinationWallet.Id, amount, currency);
    }

    // --- metodo publico controlado para acreditar ---
    public void ApplyCredit(decimal amount, CurrencyCode currency)
    {
        if (currency != CurrencyCode) throw new InvalidOperationException("la moneda de la transaccion no coincide con la wallet.");
        if (amount <= 0) throw new InvalidOperationException("El monto debe ser mayor a cero.");
        Credit(amount);
    }
}


