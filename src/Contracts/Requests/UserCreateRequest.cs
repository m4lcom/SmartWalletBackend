using Contracts.Responses;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class UserCreateRequest
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(200, ErrorMessage = "El correo no puede exceder 200 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Regular;
    }
}