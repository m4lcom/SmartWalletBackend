using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;

namespace SmartWallet.Application.Services
{
    public interface ITransactionService
    {
        // --- consultas ---
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<List<Transaction>> GetByWalletAsync(Guid walletId);
        Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<bool> ExistsAsync(Guid id);

        // --- operaciones de dominio ---
        Task<Transaction> CreateDepositAsync(Guid walletId, decimal amount, CurrencyCode currency);
        Task<Transaction> CreateWithdrawalAsync(Guid walletId, decimal amount, CurrencyCode currency);
        Task<Transaction> CreateTransferAsync(Guid sourceWalletId, Guid destinationWalletId, decimal amount, CurrencyCode currency);
        Task<Transaction> MarkAsFailedAsync(Guid transactionId);
        Task<Transaction> MarkAsCanceledAsync(Guid transactionId);
    }
}
