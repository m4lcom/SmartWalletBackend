# dbcontext-setup

## Objetivo  
Establecer el `SmartWalletDbContext` como base de persistencia para el proyecto, permitiendo registrar entidades del dominio, configurar relaciones explícitas y preparar el contexto para migraciones y repositorios concretos.

---

## Ubicación  
`SmartWallet.Infrastructure.Persistence.Context.SmartWalletDbContext`

---

## Estructura del archivo  
```csharp
public class SmartWalletDbContext : DbContext
{
    // TODO: agregar DbSet<User>
    public DbSet<User> Users { get; set; } = null!;

    // TODO: agregar DbSet<Wallet>
    public DbSet<Wallet> Wallets { get; set; } = null!;

    // TODO: agregar DbSet<Transaction>
    public DbSet<Transaction> Transactions { get; set; } = null!;

    // TODO: agregar DbSet<TransactionLedger>
    public DbSet<TransactionLedger> TransactionLedgers { get; set; } = null!;

    public SmartWalletDbContext(DbContextOptions<SmartWalletDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TODO: configurar relaciones entre Wallet y Transaction
        // modelBuilder.Entity<Wallet>()
        //     .HasMany(w => w.Transactions)
        //     .WithOne(t => t.Wallet)
        //     .HasForeignKey(t => t.WalletId);

        // TODO: configurar relación opcional DestinationWallet
        // modelBuilder.Entity<Transaction>()
        //     .HasOne(t => t.DestinationWallet)
        //     .WithMany()
        //     .HasForeignKey(t => t.DestinationWalletId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
