using SmartWallet.Application.Abstractions;
using SmartWallet.Domain.Entities;


namespace SmartWallet.Application.Services
{
    public class TransactionLedgerService : ITransactionLedgerService
    {
        private readonly ITransactionLedgerRepository _ledgerRepository;

        public TransactionLedgerService(ITransactionLedgerRepository ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }

        // --- consultas ---
        public async Task<TransactionLedger?> GetByIdAsync(Guid id) => await _ledgerRepository.GetByIdAsync(id);

        public async Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId) => await _ledgerRepository.GetByWalletAsync(walletId);

        public async Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId) => await _ledgerRepository.GetByTransactionAsync(transactionId);

        public async Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to) => await _ledgerRepository.GetByDateRangeAsync(from, to);

        public async Task<bool> ExistsAsync(Guid id) => await _ledgerRepository.ExistsAsync(id);

        // --- operaciones de dominio ---
        public async Task<TransactionLedger> CreateReversalAsync(Guid ledgerId, string? metadata = null)
        {
            var ledger = await _ledgerRepository.GetByIdAsync(ledgerId) ?? throw new KeyNotFoundException("Ledger no encontrado");
            var reversal = ledger.CreateReversal(metadata);
            await _ledgerRepository.AddAsync(reversal);
            return reversal;
        }

        public async Task<TransactionLedger> CreateAdjustmentAsync(Guid ledgerId, decimal newAmount, string? metadata = null)
        {
            var ledger = await _ledgerRepository.GetByIdAsync(ledgerId) ?? throw new KeyNotFoundException("Ledger no encontrado");
            var adjustment = ledger.CreateAdjustment(newAmount, metadata);
            await _ledgerRepository.AddAsync(adjustment);
            return adjustment;
        }
    }
}
