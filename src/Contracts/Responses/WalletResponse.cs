namespace Contracts.Responses
{
    public class WalletResponse
    {
        public Guid Id  { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
