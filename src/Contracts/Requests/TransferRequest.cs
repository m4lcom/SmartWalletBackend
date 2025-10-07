using System.ComponentModel.DataAnnotations;

namespace SmartWallet.Contracts.Requests
{
    public class TransferRequest
    {
        [Required]
        public Guid SourceWalletId { get; init; }

        [Required]
        public Guid DestinationWalletId { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Amount { get; init; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código de moneda debe tener 3 caracteres (ISO 4217).")]
        public string CurrencyCode { get; init; } = string.Empty;
    }
}
