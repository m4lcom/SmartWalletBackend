using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstraction;
using SmartWallet.Domain.Entities;


namespace SmartWallet.Infrastructure.Persistence.Repositories;

public class UserRepository:BaseRepository<User>, IUserRepository
{
    private readonly SmartWalletDbContext _context;
   
    protected UserRepository(SmartWalletDbContext context):base(context)
    {
        _context = context;
    }

    public User? GetUserByEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        return user;
    }
}
