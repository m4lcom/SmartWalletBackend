using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;


namespace SmartWallet.Application.Services
{
    public interface IWalletService
    {
        public IEnumerable<Wallet> GetAll();
        public Wallet Create(Guid userId, string name, CurrencyCode currency, string alias, decimal initialBalance = 0);

        public Wallet? GetById(Guid id);
        public void Delete(Guid id);
    }

}