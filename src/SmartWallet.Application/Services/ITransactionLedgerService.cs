using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Services
{
    public interface ITransactionLedgerService
    {
        // --- consultas ---
        Task<TransactionLedger?> GetByIdAsync(Guid id);
        Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId);
        Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId);
        Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<bool> ExistsAsync(Guid id);

        // --- operaciones de dominio ---
        Task<TransactionLedger> CreateReversalAsync(Guid ledgerId, string? metadata = null);
        Task<TransactionLedger> CreateAdjustmentAsync(Guid ledgerId, decimal newAmount, string? metadata = null);
    }
}
