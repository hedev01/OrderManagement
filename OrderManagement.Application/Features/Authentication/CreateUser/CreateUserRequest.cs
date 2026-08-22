using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Authentication.CreateUser
{
    public sealed class CreateUserRequest
    {
        public string Username { get; init; } = null!;

        public string Password { get; init; } = null!;

        public UserRole Role { get; init; }
    }
}
