﻿using Contracts.Responses;

namespace Contracts.Requests
{
    public class UserCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Regular;
    }
}
