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

        public async Task<TransactionLedger?> GetByIdAsync(Guid id) => await _ledgerRepository.GetByIdAsync(id);

        public async Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId) => await _ledgerRepository.GetByWalletAsync(walletId);

        public async Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId) => await _ledgerRepository.GetByTransactionAsync(transactionId);

        public async Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to) => await _ledgerRepository.GetByDateRangeAsync(from, to);

        public async Task<bool> ExistsAsync(Guid id) => await _ledgerRepository.ExistAsync(id);

        public async Task<TransactionLedger> MarkAsCompletedAsync(Guid id)
        {
            var ledger = await _ledgerRepository.GetByIdAsync(id);
            if (ledger == null) throw new KeyNotFoundException("Ledger no encontrado");
            ledger.MarkAsCompleted();
            await _ledgerRepository.UpdateAsync(ledger);
            return ledger;
        }

        public async Task<TransactionLedger> MarkAsFailedAsync(Guid id)
        {
            var ledger = await _ledgerRepository.GetByIdAsync(id);
            if (ledger == null) throw new KeyNotFoundException("Ledger no encontrado");
            ledger.MarkAsFailed();
            await _ledgerRepository.UpdateAsync(ledger);
            return ledger;
        }

        public async Task<TransactionLedger> MarkAsCanceledAsync(Guid id)
        {
            var ledger = await _ledgerRepository.GetByIdAsync(id);
            if (ledger == null) throw new KeyNotFoundException("Ledger no encontrado");
            ledger.MarkAsCanceled();
            await _ledgerRepository.UpdateAsync(ledger);
            return ledger;
        }
    }
}
