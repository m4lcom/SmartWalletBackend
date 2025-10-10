using SmartWallet.Application.Abstraction;
using SmartWallet.Contracts.Requests;
using SmartWallet.Contracts.Responses;


namespace SmartWallet.Application.Services
{
    public class UserServices: IUserServices
    {
        private readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserResponse> GetAllUsers()
        {
            var users = _userRepository.GetAll();
            var userResponses = users.Select(user => new UserResponse
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active
            }).ToList();
            return userResponses;
        }

        public UserResponse? GetUserById(Guid id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return null;
            return new UserResponse
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active
            };
        }

        public UserResponse? GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return null;
            return new UserResponse
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active
            };
        }

        public bool CreateUser (UserCreateRequest request)
        {
            var existingUser = _userRepository.GetUserByEmail(request.Email);
            if (existingUser != null) 
                return false;
            var passwordHash = request.Password;
            var newUser = new Domain.Entities.User(
                request.Name,
                request.Email,
                passwordHash,
                (Domain.Enums.UserRole)request.Role,
                true
            );
            _userRepository.Create(newUser);
            return true;
        }

        public bool UpdateUser(string email, UserUpdateDataRequest request)
        {
            var user = _userRepository.GetUserByEmail(email);
            
            if (user == null) 
                return false;
            
            if (!string.IsNullOrWhiteSpace(request.Name))
                user.ChangeName(request.Name);
           
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var passwordHash = request.Password;
                user.ChangePassword(passwordHash);
            }
            
            if(request.Role != null)
                user.ChangeRole((Domain.Enums.UserRole)request.Role);
            
            if(request.Active != null)
                user.SetActive(request.Active.Value);

            _userRepository.Update(user);
            return true;
        }

        public bool DeleteUser(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            
            if (user == null) 
                return false;
            
            _userRepository.Delete(user);
            return true;
        }

    }
}
