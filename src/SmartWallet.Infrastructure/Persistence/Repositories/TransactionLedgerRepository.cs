using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstractions;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Persistence.Repositories
{
    public class TransactionLedgerRepository : ITransactionLedgerRepository
    {
        private readonly SmartWalletDbContext _context;

        public TransactionLedgerRepository(SmartWalletDbContext context)
        { 
            _context = context;
        }

        public async Task AddAsync(TransactionLedger ledger)
        {
            await _context.TransactionLedgers.AddAsync(ledger);
            await _context.SaveChangesAsync();  
        }

        public async Task AddRangeAsync(IEnumerable<TransactionLedger> ledgers)
        {
            await _context.TransactionLedgers.AddRangeAsync(ledgers);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(Guid id)
        
         => await _context.TransactionLedgers.AnyAsync(l => l.Id == id);
        

        public async Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to)
          => await _context.TransactionLedgers.Where(l => l.Timestamp >= from && l.Timestamp <= to).ToListAsync();            
        

        public async Task<TransactionLedger?> GetByIdAsync(Guid id)
        => await _context.TransactionLedgers.Include(l => l.SourceWallet).Include(l => l.DestinationWallet).FirstOrDefaultAsync(l => l.Id == id);

        public async Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId) => await _context.TransactionLedgers.Where(l => l.SourceTransactionId == transactionId || l.DestinationTransactionId == transactionId).ToListAsync();



        public async Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId)
        => await _context.TransactionLedgers.Where(l => l.SourceWalletId == walletId || l.DestinationWalletId == walletId).OrderByDescending(l => l.Timestamp).ToListAsync();

        public async Task UpdateAsync(TransactionLedger ledger)
        {
            _context.TransactionLedgers.Update(ledger);
            await _context.SaveChangesAsync();
        }
    

    }
}
