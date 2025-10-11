
using System.ComponentModel.DataAnnotations;

namespace SmartWallet.Contracts.Requests
{
    public class DepositRequest
    {
        [Required]
        public Guid WalletId { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe sesr mayor a cero.")]
        public decimal Amount { get; init; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El codigo de la moneda debe tener 3 caracteres.")]
        public string CurrencyCode { get; init; } = string.Empty;
    }
}
