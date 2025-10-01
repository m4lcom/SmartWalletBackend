using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Wallet? GetById(Guid id);
        IEnumerable<Wallet> GetAll();
        void Add(Wallet wallet);
        void Update(Wallet wallet);
        void Delete(Guid id);
    }
}
