using Contracts.Requests;
using SmartWallet.Domain.Entities;

namespace SmartWallet.Application.Abstractions;
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
}
