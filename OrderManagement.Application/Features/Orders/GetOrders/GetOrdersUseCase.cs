using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderManagement.Application.Features.Orders.GetOrders
{

    public sealed class GetOrdersUseCase: IGetOrdersUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersUseCase(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<
            Result<PagedResult<GetOrdersResponse>>>
            ExecuteAsync(
                GetOrdersRequest request,
                CancellationToken cancellationToken)
        {
            if (request.Page <= 0)
            {
                return Result<
                    PagedResult<GetOrdersResponse>>
                    .Failure("صفحه باید بزرگتر از صفر باشد.");
            }

            if (request.PageSize <= 0 ||
                request.PageSize > 100)
            {
                return Result<
                    PagedResult<GetOrdersResponse>>
                    .Failure("اندازه صفحه باید بین ۱ تا ۱۰۰ باشد.");
            }

            if (request.FromDate.HasValue &&
                request.ToDate.HasValue &&
                request.FromDate > request.ToDate)
            {
                return Result<
                    PagedResult<GetOrdersResponse>>
                    .Failure("مقدار From Date نمی بزرگتر از ToDate باشد.");
            }

            var result =
                await _orderRepository.SearchAsync(
                    request.CustomerId,
                    request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.Page,
                    request.PageSize,
                    cancellationToken);

            var response =
                result.Items
                    .Select(order =>
                        new GetOrdersResponse(
                            order.Id,
                            order.CustomerId,
                            order.Status,
                            order.TotalPrice,
                            order.CreatedAt))
                    .ToList();

            var pagedResult =
                new PagedResult<GetOrdersResponse>(
                    response,
                    request.Page,
                    request.PageSize,
                    result.TotalCount);

            return Result<
                PagedResult<GetOrdersResponse>>
                .Success(pagedResult);
        }
    }
}
