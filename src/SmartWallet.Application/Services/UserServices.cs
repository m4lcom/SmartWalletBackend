using Contracts.Requests;
using Contracts.Responses;
using SmartWallet.Application.Abstractions;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWallet.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletService _walletService;

        public UserServices(IUserRepository userRepository, IWalletService walletService)
        {
            _userRepository = userRepository;
            _walletService = walletService;
        }

        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userResponses = users.Select(user => MapToResponse(user)).ToList();
            return userResponses;
        }

        public async Task<UserResponse?> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            return MapToResponse(user);
        }

        public async Task<UserResponse?> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return null;
            return MapToResponse(user);
        }

        // Ahora devuelve UserwithWalletResponse indicando la wallet creada (si aplica)
        public async Task<UserwithWalletResponse?> RegisterUser(UserRegisterRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return null;

            var passwordHash = request.Password;
            var newUser = new Domain.Entities.User(
                request.Name,
                request.Email,
                passwordHash,
                role: SmartWallet.Domain.Enums.UserRole.Regular,
                true
            );

            var created = await _userRepository.CreateAsync(newUser);
            if (!created) return null;

            Guid walletId = Guid.Empty;
            string walletAlias = string.Empty;

            if (newUser.Role != SmartWallet.Domain.Enums.UserRole.Admin)
            {
                try
                {
                    var alias = GenerateAlias(newUser.Name, newUser.Email);
                    var wallet = await _walletService.CreateAsync(newUser.Id, $"{newUser.Name} - Principal", SmartWallet.Domain.Enums.CurrencyCode.ARS, alias, 0m);
                    if (wallet != null)
                    {
                        walletId = wallet.Id;
                        walletAlias = wallet.Alias;
                    }
                }
                catch
                {
                    // rollback simple: eliminar usuario creado para evitar huérfanos
                    try { await _userRepository.DeleteAsync(newUser); } catch { /* swallow */ }
                    return null;
                }
            }

            return MapToUserWithWalletResponse(newUser, walletId, walletAlias);
        }

        public async Task<UserResponse?> CreateAdminUser(UserCreateRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return null;

            var passwordHash = request.Password;
            var newUser = new Domain.Entities.User(
                request.Name,
                request.Email,
                passwordHash,
                (Domain.Enums.UserRole)request.Role,
                true
            );

            var created = await _userRepository.CreateAsync(newUser);
            if (!created) return null;

            // crear wallet sólo si el rol no es Admin
            if (newUser.Role != SmartWallet.Domain.Enums.UserRole.Admin)
            {
                try
                {
                    var alias = GenerateAlias(newUser.Name, newUser.Email);
                    await _walletService.CreateAsync(newUser.Id, $"{newUser.Name} - Principal", SmartWallet.Domain.Enums.CurrencyCode.ARS, alias, 0m);
                }
                catch
                {
                    try { await _userRepository.DeleteAsync(newUser); } catch { /* swallow */ }
                    return null;
                }
            }

            return MapToResponse(newUser);
        }

        public async Task<UserResponse?> UpdateUser(Guid id, UserUpdateDataRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Name))
                user.ChangeName(request.Name);

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.ChangePassword(request.Password);

            if (request.Active != null)
                user.SetActive(request.Active.Value);

            var updated = await _userRepository.UpdateAsync(user);
            if (!updated) return null;

            return MapToResponse(user);
        }

        public async Task<UserResponse?> ChangeUserActiveStatus(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            user.SetActive(!user.Active);
            var updated = await _userRepository.UpdateAsync(user);
            if (!updated) return null;
            return MapToResponse(user);
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;
            user.SetActive(false);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        // Helper: generar alias válido (6-20 chars, sólo letras y puntos)
        private string GenerateAlias(string name, string email)
        {
            var source = string.IsNullOrWhiteSpace(name) ? (email?.Split('@')[0] ?? "user") : name;
            var normalized = source.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat == UnicodeCategory.UppercaseLetter || cat == UnicodeCategory.LowercaseLetter)
                    sb.Append(ch);
                else if (char.IsWhiteSpace(ch))
                    sb.Append('.');
                else if (ch == '.')
                    sb.Append('.');
            }

            var s = sb.ToString().ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z.]", "");
            s = Regex.Replace(s, @"\.{2,}", ".");
            s = s.Trim('.');
            if (s.Length < 6) s = s + new string('x', 6 - s.Length);
            if (s.Length > 20) s = s.Substring(0, 20);
            return s;
        }

        private UserResponse MapToResponse(Domain.Entities.User user)
        {
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

        private UserwithWalletResponse MapToUserWithWalletResponse(Domain.Entities.User user, Guid walletId, string walletAlias)
        {
            return new UserwithWalletResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = (UserRole)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Active = user.Active,
                WalletId = walletId,
                WalletAlias = walletAlias ?? string.Empty
            };
        }
    }
}