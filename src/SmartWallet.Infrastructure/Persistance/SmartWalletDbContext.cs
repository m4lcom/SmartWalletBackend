using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Persistance
{
    public class SmartWalletDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionLedger> TransactionLedgers { get; set; }

        public SmartWalletDbContext(DbContextOptions<SmartWalletDbContext> options) : base(options)
        {

        }
    }
}