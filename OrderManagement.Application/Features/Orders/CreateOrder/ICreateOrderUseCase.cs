using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Orders.CreateOrder
{

    public interface ICreateOrderUseCase
    {
        Task<Result<CreateOrderResponse>> ExecuteAsync(
            CreateOrderRequest request,
            CancellationToken cancellationToken);
    }
}
