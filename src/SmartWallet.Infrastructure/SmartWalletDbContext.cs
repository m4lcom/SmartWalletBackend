using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;
using SmartWallet.Infrastructure.Extensions;


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
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(t => t.Wallet)
                      .WithMany()
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

                entity.HasOne(l => l.Wallet)
                      .WithMany()
                      .HasForeignKey(l => l.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Seed(); // Llamada al método de extensión para sembrar datos
        }
    }
}
