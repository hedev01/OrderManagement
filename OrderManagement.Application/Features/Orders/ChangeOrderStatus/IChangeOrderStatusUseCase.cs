using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Orders.ChangeOrderStatus
{
    public interface IChangeOrderStatusUseCase
    {
        Task<Result<ChangeOrderStatusResponse>> ExecuteAsync(
            ChangeOrderStatusRequest request,
            CancellationToken cancellationToken);
    }
}
