using Contracts.Responses;


namespace SmartWallet.Application.Abstractions
{
    public interface IDolarApiService
    {
        Task<DolarDto?> GetDolarByTypeAsync(string type);
    }
}
