using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Abstractions
{
    public interface ITransactionLedgerRepository
    {
        Task AddAsync(TransactionLedger ledger);
        Task AddRangeAsync(IEnumerable<TransactionLedger> ledgers);
        Task<TransactionLedger?> GetByIdAsync(Guid id);
        Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId);
        Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId);
        Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<bool> ExistsAsync(Guid ledgerId);

    }
}
