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

        // --- persiste una nueva transaccion ---
        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync(); 
        }

        // --- obtiene una transaccion por id ---
        public async Task<Transaction?> GetByIdAsync(Guid transactionId)
        {
            return await _context.Transactions.Include(t => t.Wallet).Include(t => t.DestinationWallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        // --- obtiene todas las transacciones asociadas a una wallet especifica ---
        public async Task<List<Transaction>> GetByWalletAsync(Guid walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId).OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        // --- obtiene todas las transacciones dentro de un rango de fechas ---
        public async Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Transactions
                .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        // --- verifica si existe una transaccion con un id especifico ---
        public async Task<bool> ExistsAsync(Guid transactionId)
        {
            return await _context.Transactions.AnyAsync(t => t.Id == transactionId);
        }

        // --- actualiza el estado de una transaccion existente ---
        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

    }
}
