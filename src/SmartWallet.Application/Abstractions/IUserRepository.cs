using SmartWallet.Domain.Entities;


namespace SmartWallet.Application.Abstractions
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User? GetUserByEmail(string email);
    }
}
