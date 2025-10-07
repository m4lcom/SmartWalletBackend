using SmartWallet.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SmartWalletDbContext _context;
        public TransactionRepository(SmartWalletDbContext context) 
        {
            _context = context;
        }
        public async Task<Transaction?> GetByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.Include(t => t.Wallet).Include(t => t.DestinationWallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<List<Transaction>> GetByWalletAsync(Guid walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId).OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Transactions
                .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Transaction transaction)
        {
            var ledgerEntries = TransactionLedger.FromTransaction(transaction);
            await _context.TransactionLedgers.AddRangeAsync(ledgerEntries);
        }

        public async Task<bool> ExistsAsync(Guid transactionId)
        {
            return await _context.TransactionLedgers.AnyAsync(t => t.SourceTransactionId == transactionId || t.DestinationTransactionId == transactionId);
        }

    }
}
