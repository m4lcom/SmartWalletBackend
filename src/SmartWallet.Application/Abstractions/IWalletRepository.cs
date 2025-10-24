using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Abstractions.Persistence
{
    public interface IWalletRepository
    {
        // --- consultas ---
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Wallet?> GetByAliasAsync(string alias);

        // --- operaciones ---
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
    }
}
