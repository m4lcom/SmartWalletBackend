using SmartWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWallet.Application.Abstractions.Persistence
{
    public interface IWalletRepository
    {
        // --- consultas ---
        Task<List<Wallet>> GetAllAsync();
        Task<List<Wallet>> GetByUserIdAsync(Guid userId);
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Wallet?> GetByAliasAsync(string alias);

        // --- operaciones ---
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
    }
}
