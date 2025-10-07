using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Abstractions
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid transactionId);
        Task<List<Transaction>> GetByWalletAsync(Guid walletId);
        Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task AddAsync(Transaction transaction);
        Task<bool> ExistsAsync(Guid transactionId);
    }
}
