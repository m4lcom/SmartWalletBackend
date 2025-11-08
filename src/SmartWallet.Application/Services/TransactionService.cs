using SmartWallet.Application.Abstractions;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;

namespace SmartWallet.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionLedgerRepository _ledgerRepository;
        private readonly IWalletRepository _walletRepository;

        public TransactionService(ITransactionRepository transactionRepository, ITransactionLedgerRepository ledgerRepository, IWalletRepository walletRepository)
        {
            _transactionRepository = transactionRepository;
            _ledgerRepository = ledgerRepository;
            _walletRepository = walletRepository;
        }

        // --- consultas ---
        public async Task<Transaction?> GetByIdAsync(Guid id)
            => await _transactionRepository.GetByIdAsync(id);

        public async Task<List<Transaction>> GetByWalletAsync(Guid walletId)
            => await _transactionRepository.GetByWalletAsync(walletId);

        public async Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to) => await _transactionRepository.GetByDateRangeAsync(from, to);

        public async Task<bool> ExistsAsync(Guid id) => await _transactionRepository.ExistsAsync(id);

        // --- operaciones de dominio --- 
        public async Task<Transaction> CreateDepositAsync(Guid walletId, decimal amount, CurrencyCode currency)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId) ?? throw new KeyNotFoundException("Wallet no encontrada.");

            var transaction = wallet.Deposit(amount, currency);

            await _transactionRepository.AddAsync(transaction);
            var ledgers = TransactionLedger.FromTransaction(transaction);
            await _ledgerRepository.AddRangeAsync(ledgers);

            transaction.MarkAsCompleted();
            await _transactionRepository.UpdateAsync(transaction);
            await _walletRepository.UpdateAsync(wallet);
            return transaction;
        }

        public async Task<Transaction> CreateWithdrawalAsync(Guid walletId, decimal amount, CurrencyCode currency)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId) ?? throw new KeyNotFoundException("Wallet no encontrada.");

            var transaction = wallet.Withdrawal(amount, currency);

            await _transactionRepository.AddAsync(transaction);
            var ledgers = TransactionLedger.FromTransaction(transaction);
            await _ledgerRepository.AddRangeAsync(ledgers);

            transaction.MarkAsCompleted();

            await _transactionRepository.UpdateAsync(transaction);
            await _walletRepository.UpdateAsync(wallet);

            return transaction;
        }

        public async Task<Transaction> CreateTransferAsync(Guid sourceWalletId, Guid destinationWalletId, decimal amount, CurrencyCode currency)
        {
            var sourceWallet = await _walletRepository.GetByIdAsync(sourceWalletId) ?? throw new KeyNotFoundException("Wallet origen no encontrada.");

            var destinationWallet = await _walletRepository.GetByIdAsync(destinationWalletId) ?? throw new KeyNotFoundException("Wallet destino no encontrada.");
            
            // --- logica de dominio: debitar origen y acreditar destino ---
            var transaction = sourceWallet.Transfer(destinationWallet, amount, currency);

            // --- persistencia ---
            await _transactionRepository.AddAsync(transaction);
            var ledgers = TransactionLedger.FromTransaction(transaction);
            await _ledgerRepository.AddRangeAsync(ledgers);

            transaction.MarkAsCompleted();

            await _transactionRepository.UpdateAsync(transaction);
            await _walletRepository.UpdateAsync(sourceWallet);
            await _walletRepository.UpdateAsync(destinationWallet);
            return transaction;
        }

        public async Task<Transaction> MarkAsFailedAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId)
                ?? throw new KeyNotFoundException("Transacción no encontrada");

            transaction.MarkAsFailed();
            await _transactionRepository.UpdateAsync(transaction);

            return transaction;
        }

        public async Task<Transaction> MarkAsCanceledAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId)
                ?? throw new KeyNotFoundException("Transacción no encontrada");

            transaction.MarkAsCanceled();
            await _transactionRepository.UpdateAsync(transaction);

            return transaction;
        }
    }
}
