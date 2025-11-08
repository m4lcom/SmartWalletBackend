
using Contracts.Requests;
using Contracts.Responses;
using SmartWallet.Application.Abstractions;


namespace SmartWallet.Application.Services
{
    public class UserServices: IUserServices
    {
        private readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userResponses = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active
            }).ToList();
            return userResponses;
        }

        public async Task<UserResponse?> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active
            };
        }

        public async Task<UserResponse?> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return null;
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active
            };
        }
        public async Task<bool> RegisterUser(UserCreateRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return false;
            var passwordHash = request.Password;
            var newUser = new Domain.Entities.User(
                request.Name,
                request.Email,
                passwordHash,
                role: Domain.Enums.UserRole.Regular,
                true
            );
            await _userRepository.CreateAsync(newUser);
            return true;
        }

        public async Task<bool> CreateUser (UserCreateRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
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
            await _userRepository.CreateAsync(newUser);
            return true;
        }

        public async Task<bool> UpdateUser(Guid id, UserUpdateDataRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);

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

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangeUserActiveStatus(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;
            user.SetActive(!user.Active);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) 
                return false;
            user.SetActive(false);

            await _userRepository.UpdateAsync(user);
            return true;
        }

    }
}
