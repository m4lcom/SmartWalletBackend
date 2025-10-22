using SmartWallet.Application.Abstractions.Persistence;
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

        public Task<Wallet?> GetByIdAsync(Guid id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<Wallet> CreateAsync(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance = 0)
        {
            var wallet = new Wallet(userId, name, currencyCode, alias, initialBalance);
            await _repository.AddAsync(wallet); // ahora sí esperás la operación asíncrona
            return wallet;
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Wallet wallet)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Wallet wallet)
        {
            throw new NotImplementedException();
        }


    }
}
