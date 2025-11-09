using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWallet.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _repository;

        public WalletService(IWalletRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Wallet>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Wallet>> GetByUserIdAsync(Guid userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<Wallet?> GetByAliasAsync(string alias)
        {
            return await _repository.GetByAliasAsync(alias);
        }

        public Task<Wallet?> GetByIdAsync(Guid id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<Wallet> CreateAsync(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance = 0)
        {
            var wallet = new Wallet(userId, name, currencyCode, alias, initialBalance);
            await _repository.AddAsync(wallet);
            return wallet;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task AddAsync(Wallet wallet)
        {
            await _repository.AddAsync(wallet);
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            await _repository.UpdateAsync(wallet);
        }
    }
}
