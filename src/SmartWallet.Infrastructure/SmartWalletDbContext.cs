using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;


namespace SmartWallet.Infrastructure
{
    public class SmartWalletDbContext : DbContext
    {
        // agregar los DbSet para tus entidades
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionLedger> TransactionLedgers { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<User> Users { get; set; }

        public SmartWalletDbContext(DbContextOptions<SmartWalletDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- transaction ---
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Amount)
                      .HasConversion<double>();

                entity.HasOne(t => t.Wallet)
                      .WithMany(w => w.Transactions)
                      .HasForeignKey(t => t.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.DestinationWallet)
                      .WithMany(w => w.ReceivedTransfers)
                      .HasForeignKey(t => t.DestinationWalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- transactionLedger ---
            modelBuilder.Entity<TransactionLedger>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Amount)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(l => l.SourceWallet)
                      .WithMany(w => w.SourceLedgers)
                      .HasForeignKey(l => l.SourceWalletId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.DestinationWallet)
                      .WithMany(w => w.DestinationLedgers)
                      .HasForeignKey(l => l.DestinationWalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
