

using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Services
{
    public interface IWalletService
    {
        // --- consultas ---
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<List<Wallet>> GetAllByUserAsync(Guid userId);
        Task<bool> ExistsAsync(Guid id);

        // --- operaciones ---
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task DeleteAsync(Wallet wallet);
    }
}
