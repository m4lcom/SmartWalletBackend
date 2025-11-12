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

            // --- Transaction ---
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(t => t.CurrencyCode).IsRequired();
                entity.Property(t => t.Type).IsRequired();
                entity.Property(t => t.Status).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();

                // relación con Wallet origen
                entity.HasOne(t => t.Wallet)
                      .WithMany(w => w.Transactions)
                      .HasForeignKey(t => t.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);

                // relación con Wallet destino
                entity.HasOne(t => t.DestinationWallet)
                      .WithMany(w => w.ReceivedTransfers)
                      .HasForeignKey(t => t.DestinationWalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- TransactionLedger ---
            modelBuilder.Entity<TransactionLedger>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(l => l.CurrencyCode).IsRequired();
                entity.Property(l => l.Type).IsRequired();
                entity.Property(l => l.Status).IsRequired();
                entity.Property(l => l.Timestamp).IsRequired();

                // relación con Wallet
                entity.HasOne(l => l.Wallet)
                      .WithMany(w => w.TransactionLedgers)
                      .HasForeignKey(l => l.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- seed inicial ---
            modelBuilder.Seed();
        }



    }
}
