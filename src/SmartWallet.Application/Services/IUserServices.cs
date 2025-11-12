using Contracts.Requests;
using Contracts.Responses;


namespace SmartWallet.Application.Services
{
    public interface IUserServices
    {
        public Task<List<UserResponse>> GetAllUsers();
        public Task<UserResponse?> GetUserById(Guid id);
        public Task<UserResponse?> GetUserByEmail(string email);
        public Task<UserwithWalletResponse?> RegisterUser(UserRegisterRequest request);
        public Task<UserResponse?> CreateAdminUser(UserCreateRequest request);
        public Task<UserResponse?> UpdateUser(Guid id, UserUpdateDataRequest request);
        public Task<UserResponse?> ChangeUserActiveStatus(Guid id);
        public Task<bool> DeleteUser(Guid id);
    }
}
