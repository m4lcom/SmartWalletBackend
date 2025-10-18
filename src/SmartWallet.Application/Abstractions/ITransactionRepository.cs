using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;

namespace SmartWallet.Application.Abstractions
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(Guid transactionId);
        Task<List<Transaction>> GetByWalletAsync(Guid walletId);
        Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<bool> ExistsAsync(Guid transactionId);
        Task UpdateAsync(Transaction transaction);  
    }
}
