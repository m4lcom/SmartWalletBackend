
namespace Contracts.Responses
{
    public record TransactionResponse(
        Guid id,
        DateTimeOffset CreatedAt,
        string Type,
        decimal Amount,
        string CurrencyCode,
        string Status,
        Guid WalletId,
        Guid? DestinationWalletId
        );
}
