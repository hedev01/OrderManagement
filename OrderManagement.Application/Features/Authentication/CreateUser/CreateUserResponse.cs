using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Authentication.CreateUser
{
    public sealed class CreateUserResponse
    {
        public Guid Id { get; init; }

        public string Username { get; init; } = null!;

        public string Role { get; init; } = null!;
    }
}
