using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Services
{
    public interface ITransactionLedgerService
    {
        Task<TransactionLedger?> GetByIdAsync(Guid id);
        Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId);
        Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId);
        Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to);

        Task<bool> ExistsAsync(Guid id);

        Task <TransactionLedger> MarkAsCompletedAsync(Guid id);
        Task <TransactionLedger> MarkAsFailedAsync(Guid id);
        Task <TransactionLedger> MarkAsCanceledAsync(Guid id);
    }
}
