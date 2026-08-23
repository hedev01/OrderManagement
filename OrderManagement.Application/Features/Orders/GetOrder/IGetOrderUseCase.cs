using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Orders.GetOrder
{
    public interface IGetOrderUseCase
    {
        Task<Result<GetOrderResponse>> ExecuteAsync(
            GetOrderRequest request,
            CancellationToken cancellationToken);
    }
}
