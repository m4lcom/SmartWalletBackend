using Contracts.Requests;
using Contracts.Responses;


namespace SmartWallet.Application.Services
{
    public interface IUserServices
    {
        public Task<List<UserResponse>> GetAllUsers();
        public Task<UserResponse?> GetUserById(Guid id);
        public Task<UserResponse?> GetUserByEmail(string email);
        public Task<bool> RegisterUser(UserCreateRequest request)
        public Task<bool> CreateUser(UserCreateRequest request);
        public Task<bool> UpdateUser(Guid id, UserUpdateDataRequest request);
        public Task<bool> ChangeUserActiveStatus(Guid id);
        public Task<bool> DeleteUser(Guid id);
    }
}
