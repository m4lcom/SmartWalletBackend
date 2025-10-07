using SmartWallet.Application.Abstractions;
using SmartWallet.Domain.Entities;
using SmartWallet.Domain.Enums;

namespace SmartWallet.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> DepositAsync(Guid walletId, decimal amount, string currencyCode)
        {
            var transaction = Transaction.CreateDeposit(walletId, amount, Enum.Parse<CurrencyCode>(currencyCode));
            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }

        public async Task<Transaction> WithdrawAsync(Guid walletId, decimal amount, string currencyCode)
        {
            var transaction = Transaction.CreateWithdrawal(walletId, amount, Enum.Parse<CurrencyCode>(currencyCode));
            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }

        public async Task<Transaction> TransferAsync(Guid sourceWalletId, Guid destinationWalletId, decimal amount, string currencyCode)
        {
            var transaction = Transaction.CreateTransfer(sourceWalletId, destinationWalletId, amount, Enum.Parse<CurrencyCode>(currencyCode));
            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
            => await _transactionRepository.GetByIdAsync(id);

        public async Task<List<Transaction>> GetByWalletAsync(Guid walletId)
            => await _transactionRepository.GetByWalletAsync(walletId);
    }
}
