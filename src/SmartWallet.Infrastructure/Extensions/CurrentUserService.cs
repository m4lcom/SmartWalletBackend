using Microsoft.AspNetCore.Http;
using SmartWallet.Application.Abstractions;
using System.Security.Claims;

namespace SmartWallet.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }
    }
}
