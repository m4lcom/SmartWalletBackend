using SmartWallet.Application.Interfaces;
using SmartWallet.Domain.Entities;
using SmartWallet.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace SmartWallet.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly SmartWalletDbContext _context;

        public WalletRepository(SmartWalletDbContext context)
        {
            _context = context;
        }

        public Wallet? GetById(Guid id) =>
            _context.Wallets.FirstOrDefault(w => w.WalletID == id);

        public IEnumerable<Wallet> GetAll() =>
            _context.Wallets.ToList();

        public void Add(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
        }

        public void Update(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var wallet = _context.Wallets.FirstOrDefault(w => w.WalletID == id);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
                _context.SaveChanges();
            }
        }
    }
}
