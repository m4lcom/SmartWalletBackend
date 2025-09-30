using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;


namespace SmartWallet.Infrastructure.Persistence.Context
{
    public class SmartWalletDbContext : DbContext
    {
        // agregar los DbSet para tus entidades

        public SmartWalletDbContext(DbContextOptions<SmartWalletDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // agregar relaciones entre entidades si es necesario
        }
    }
}
