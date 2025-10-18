namespace SmartWallet.Contracts.Responses
{
    public record TransactionLedgerResponse(
        Guid Id,
        DateTimeOffset Timestamp,
        string Type,
        decimal Amount,
        string CurrencyCode,
        string Status,
        Guid WalletId,
        Guid TransactionId,
        string? metadata
        );
    
}
