using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Authentication.CreateUser
{
    public interface ICreateUserUseCase
    {
        Task<Result<CreateUserResponse>> ExecuteAsync(
            CreateUserRequest request,
            CancellationToken cancellationToken = default);
    }
}
