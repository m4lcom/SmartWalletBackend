using SmartWallet.Application.Interfaces;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly List<Wallet> _wallets = new();

        public Wallet? GetById(Guid id) =>
            _wallets.FirstOrDefault(w => w.WalletID == id);

        public IEnumerable<Wallet> GetAll() => _wallets;

        public void Add(Wallet wallet) => _wallets.Add(wallet);

        public void Update(Wallet wallet)
        {
            var index = _wallets.FindIndex(w => w.WalletID == wallet.WalletID);
            if (index >= 0) _wallets[index] = wallet;
        }

        public void Delete(Guid id)
        {
            var wallet = _wallets.FirstOrDefault(w => w.WalletID == id);
            if (wallet != null) _wallets.Remove(wallet);
        }
    }
}
