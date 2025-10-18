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

    public User? GetUserByEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        return user;
    }
}
