using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Authentication.Login
{
    public sealed class LoginRequest
    {
        public string Username { get; init; } = null!;

        public string Password { get; init; } = null!;
    }
}
