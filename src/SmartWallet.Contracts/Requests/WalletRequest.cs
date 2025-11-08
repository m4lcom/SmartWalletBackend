

namespace SmartWallet.Contracts.Requests;

public class WalletRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CurrencyCode CurrencyCode { get; set; }
    public string Alias { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
}
