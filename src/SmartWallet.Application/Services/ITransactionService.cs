using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Services
{
    public interface ITransactionService
    {
        Task<Transaction> DepositAsync(Guid walletId, decimal amount, string currencyCode);
        Task<Transaction> WithdrawAsync(Guid walletId, decimal amount, string currencyCode);
        Task<Transaction> TransferAsync(Guid sourceWalletId, Guid destinationWalletId, decimal amount, string currencyCode);

        Task<Transaction?> GetByIdAsync(Guid id);
        Task<List<Transaction>> GetByWalletAsync(Guid walletId);
    }
}
