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

        // --- persiste un nuevo ledger ---
        public async Task AddAsync(TransactionLedger ledger)
        {
            await _context.TransactionLedgers.AddAsync(ledger);
            await _context.SaveChangesAsync();
        }

        // --- persiste multiples ledgers en una sola operacion ---
        public async Task AddRangeAsync(IEnumerable<TransactionLedger> ledgers)
        {
            await _context.TransactionLedgers.AddRangeAsync(ledgers);
            await _context.SaveChangesAsync();
        }

        // --- obtiene un ledger por su id ---
        public async Task<TransactionLedger?> GetByIdAsync(Guid id)
        => await _context.TransactionLedgers.Include(l => l.Wallet).FirstOrDefaultAsync(l => l.Id == id);

        // --- obtiene todos los ledgers asociados a una wallet especifica ---
        public async Task<List<TransactionLedger>> GetByWalletAsync(Guid walletId)
        => await _context.TransactionLedgers.Where(l => l.WalletId == walletId).OrderByDescending(l => l.Timestamp).ToListAsync();

        // --- obtiene todos los ledgers dentro de un rango de fechas ---
        public async Task<List<TransactionLedger>> GetByDateRangeAsync(DateTime from, DateTime to)
          => await _context.TransactionLedgers.Where(l => l.Timestamp >= from && l.Timestamp <= to).OrderBy(l => l.Timestamp).ToListAsync();

        // --- obtiene todos los ledgers asociados a una transaccion especifica ---
        public async Task<List<TransactionLedger>> GetByTransactionAsync(Guid transactionId) => await _context.TransactionLedgers.Where(l => l.TransactionId == transactionId).OrderByDescending(l => l.Timestamp).ToListAsync();

        // --- verifica si existe un ledger con un id especifico ---
        public async Task<bool> ExistsAsync(Guid LedgerId)

         => await _context.TransactionLedgers.AnyAsync(l => l.Id == LedgerId);




    }
}
