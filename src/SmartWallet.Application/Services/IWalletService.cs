using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWallet.Application.Services
{
    public interface IWalletService
    {
        // --- consultas ---
        Task<List<Wallet>> GetAllAsync();
        Task<List<Wallet>> GetByUserIdAsync(Guid userId);
        Task<Wallet?> GetByAliasAsync(string alias);
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        // --- operaciones ---
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task<Wallet> CreateAsync(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance = 0);
    }
}