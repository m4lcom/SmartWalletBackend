using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;


using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Services
{
    public interface IWalletService
    {
        // --- consultas ---
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        // --- operaciones ---
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task<Wallet> CreateAsync(Guid userId, string name, CurrencyCode currencyCode, string alias, decimal initialBalance);
    }

}