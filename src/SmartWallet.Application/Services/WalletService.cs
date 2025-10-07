using SmartWallet.Application.Interfaces;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;

namespace SmartWallet.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _repository;

        public WalletService(IWalletRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Wallet> GetAll() => _repository.GetAll();

        public Wallet? GetById(Guid id) => _repository.GetById(id);

        public Wallet Create(Guid userId, string name, CurrencyCode currency, string alias, decimal initialBalance = 0)
        {
            var wallet = new Wallet(userId, name, currency, alias, initialBalance);
            _repository.Add(wallet);
            return wallet;
        }

        public void Delete(Guid id) => _repository.Delete(id);
    }
}
