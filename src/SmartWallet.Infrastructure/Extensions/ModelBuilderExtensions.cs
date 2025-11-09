using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User("Albus Dumbledore", "albus@mail.com", "1234", Domain.Enums.UserRole.Admin, true),
                new User("Admin Admin", "admin@mail.com", "1234", Domain.Enums.UserRole.Admin, true)
            );
        }
    }
}
