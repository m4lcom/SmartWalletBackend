using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Infrastructure.Persistence.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly SmartWalletDbContext _context;

        public WalletRepository(SmartWalletDbContext context)
        {
            _context = context;
        }

        // --- consultas ---
        public async Task<Wallet?> GetByIdAsync(Guid id)
        {
            return await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<Wallet>> GetAllByUserAsync(Guid userId)
        {
            return await _context.Wallets
                .AsNoTracking()
                .Where(w => w.UserID == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Wallets.AnyAsync(w => w.Id == id);
        }

        // --- operaciones ---
        public async Task AddAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Wallet wallet)
        {
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }

        public Task<Wallet?> GetByAliasAsync(string alias)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Task<Wallet?> wallet)
        {
            throw new NotImplementedException();
        }
    }
}
