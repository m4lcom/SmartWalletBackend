using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using SmartWallet.Application.Abstraction;
using SmartWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWallet.Infrastructure.Persistance.Repositories
{
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
}
