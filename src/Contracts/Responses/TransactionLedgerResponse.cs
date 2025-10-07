namespace SmartWallet.Contracts.Responses
{
    public class TransactionLedgerResponse
    {
        public Guid Id { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public string Type { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string CurrencyCode { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid? SourceWalletId { get; init; }
        public Guid? DestinationWalletId { get; init; }
        public Guid? SourceTransactionId { get; init; }
        public Guid? DestinationTransactionId { get; init; }
        public string? Metadata { get; init; }

        public TransactionLedgerResponse(
            Guid id,
            DateTimeOffset timestamp,
            string type,
            decimal amount,
            string currencyCode,
            string status,
            Guid? sourceWalletId,
            Guid? destinationWalletId,
            Guid? sourceTransactionId,
            Guid? destinationTransactionId,
            string? metadata)
        {
            Id = id;
            Timestamp = timestamp;
            Type = type;
            Amount = amount;
            CurrencyCode = currencyCode;
            Status = status;
            SourceWalletId = sourceWalletId;
            DestinationWalletId = destinationWalletId;
            SourceTransactionId = sourceTransactionId;
            DestinationTransactionId = destinationTransactionId;
            Metadata = metadata;
        }
    }
}
