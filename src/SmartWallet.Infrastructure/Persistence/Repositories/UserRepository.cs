using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstractions;
using SmartWallet.Domain.Entities;


namespace SmartWallet.Infrastructure.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly SmartWalletDbContext _context;

    public UserRepository(SmartWalletDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }
}
