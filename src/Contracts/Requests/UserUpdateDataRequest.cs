using Contracts.Responses;

namespace Contracts.Requests
{
    public class UserUpdateDataRequest
    {
        public string? Name { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public bool ? Active { get; set; } = true;
    }
}
