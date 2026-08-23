using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Orders.GetOrder
{
    public sealed class GetOrderUseCase : IGetOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderUseCase(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<Result<GetOrderResponse>> ExecuteAsync(
            GetOrderRequest request,
            CancellationToken cancellationToken)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    request.Id,
                    cancellationToken);

            if (order is null)
            {
                return Result<GetOrderResponse>.Failure("سفارش یافت نشد.");
            }

            var response =
                new GetOrderResponse(
                    order.Id,
                    order.CustomerId,
                    order.Status,
                    order.TotalPrice,
                    order.CreatedAt,
                    order.Items
                        .Select(item =>
                            new GetOrderItemResponse(
                                item.ProductId,
                                item.Product.Name,
                                item.Quantity,
                                item.UnitPrice,
                                item.TotalPrice))
                        .ToList());

            return Result<GetOrderResponse>.Success(
                response);
        }
    }
}
