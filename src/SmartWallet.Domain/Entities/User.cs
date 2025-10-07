using SmartWallet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartWallet.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid UserID { get; private set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; private set; } = string.Empty;
        [Required]
        [StringLength(200, ErrorMessage = "El correo no puede exceder 200 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Wallet? Wallet { get; private set; }
        protected User() { }

        public User(string name, string email, string passwordHash, UserRole role)
        {
            UserID = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;


        }
    }
}
