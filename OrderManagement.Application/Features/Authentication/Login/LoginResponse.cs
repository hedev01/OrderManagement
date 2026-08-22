using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Authentication.Login
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = null!;
    }
}
