
namespace Contracts.Responses
{
    public class TransactionResponse
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public string Type { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string CurrencyCode { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid WalletId { get; init; }
        public Guid? DestinationWalletId { get; init; }

        public TransactionResponse(
            Guid id,
            DateTime createdAt,
            string type,
            decimal amount,
            string currencyCode,
            string status,
            Guid walletId,
            Guid? destinationWalletId)
        {
            Id = id;
            CreatedAt = createdAt;
            Type = type;
            Amount = amount;
            CurrencyCode = currencyCode;
            Status = status;
            WalletId = walletId;
            DestinationWalletId = destinationWalletId;
        }
    }
}
