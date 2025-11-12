namespace SmartWallet.Application.Abstractions
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
        bool IsAdmin();
    }
}

