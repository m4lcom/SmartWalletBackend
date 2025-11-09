using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstractions.Persistence;
using SmartWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<Wallet>> GetAllAsync()
        {
            return await _context.Wallets
                .Include(w => w.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Wallet>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .AsNoTracking()
                .Where(w => w.UserID == userId)
                .ToListAsync();
        }

        public async Task<Wallet?> GetByIdAsync(Guid id)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Wallets.AnyAsync(w => w.Id == id);
        }

        public async Task<Wallet?> GetByAliasAsync(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) return null;
            var normalized = alias.ToLowerInvariant();
            return await _context.Wallets
                .Include(w => w.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Alias == normalized);
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
    }
}
