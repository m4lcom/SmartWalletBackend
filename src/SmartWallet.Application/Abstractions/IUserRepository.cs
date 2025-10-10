using Application.Abstraction;
using SmartWallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWallet.Application.Abstraction
{
    public interface IUserRepository:IBaseRepository<User>
    
    {
        User? GetUserByEmail(string email);
    }
}
