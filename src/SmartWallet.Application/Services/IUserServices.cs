using SmartWallet.Contracts.Requests;
using SmartWallet.Contracts.Responses;


namespace SmartWallet.Application.Services
{
    public interface IUserServices
    {
        public List<UserResponse> GetAllUsers();
        public UserResponse? GetUserById(Guid id);
        public UserResponse? GetUserByEmail(string email);
        public bool CreateUser(UserCreateRequest request);
        public bool UpdateUser(Guid id, UserUpdateDataRequest request);
        public bool ChangeUserActiveStatus(Guid id);
        public bool DeleteUser(Guid id);
    }
}
