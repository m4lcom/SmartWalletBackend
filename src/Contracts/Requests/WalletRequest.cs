using System.ComponentModel.DataAnnotations;


namespace Contracts.Requests
{
    public class WalletRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string CurrencyCode { get; set; } = string.Empty;

        [Required]
        public string Alias { get; set; } = string.Empty;

        [Required]
        public decimal InitialBalance { get; set; }

    }
}
