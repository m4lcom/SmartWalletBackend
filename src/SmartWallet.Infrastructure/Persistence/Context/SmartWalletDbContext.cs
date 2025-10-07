using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Persistence.Context
{
    public class SmartWalletDbContext : DbContext
    {
        public SmartWalletDbContext(DbContextOptions<SmartWalletDbContext> options) : base(options)
        {
        }

        
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithOne(u => u.Wallet) 
                .HasForeignKey<Wallet>(w => w.UserID);
        }
    }
}
