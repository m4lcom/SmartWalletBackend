using SmartWallet.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWallet.Contracts.Requests
{
    public class UserUpdateDataRequest
    {
        public string? Name { get; set; } = string.Empty;

        public string? Password { get; set; } = string.Empty;
        public UserRole? Role { get; set; } = UserRole.Regular;

        public bool ? Active { get; set; } = true;
    }
}
