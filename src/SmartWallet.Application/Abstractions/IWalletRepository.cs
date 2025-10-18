using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Abstractions.Persistence
{
    public interface IWalletRepository
    {
        // --- consultas ---
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<List<Wallet>> GetAllByUserAsync(Guid userId);
        Task<bool> ExistsAsync(Guid id);
        Task<Wallet?> GetByAliasAsync(string alias);

        // --- operaciones ---
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task DeleteAsync(Wallet wallet);
        Task UpdateAsync(Task<Wallet?> wallet);
    }
}
