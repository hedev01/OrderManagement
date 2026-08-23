using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Orders.GetOrders
{
    public interface IGetOrdersUseCase
    {
        Task<Result<PagedResult<GetOrdersResponse>>>
            ExecuteAsync(
                GetOrdersRequest request,
                CancellationToken cancellationToken);
    }
}
