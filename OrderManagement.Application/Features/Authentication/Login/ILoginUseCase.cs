using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Authentication.Login
{
    public interface ILoginUseCase
    {
        Task<Result<LoginResponse>> ExecuteAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);
    }
}
