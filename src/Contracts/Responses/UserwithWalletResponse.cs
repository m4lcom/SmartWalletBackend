using System.Text.Json.Serialization;

namespace Contracts.Responses
{
    public class UserwithWalletResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public Guid WalletId { get; set; }
        public string WalletAlias { get; set; } = string.Empty;
    }
}
