using Contracts.Requests;

namespace SmartWallet.Application.Abstractions;

public interface IAuthenticationService
{
    Task<string?> AuthenticateAsync(LoginRequest request);
}
