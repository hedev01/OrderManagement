using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Orders.GetOrder
{
    public sealed class GetOrderUseCase : IGetOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderUseCase> _logger;

        public GetOrderUseCase(
            IOrderRepository orderRepository,
            ILogger<GetOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<GetOrderResponse>> ExecuteAsync(
            GetOrderRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting order {OrderId}",
                request.Id);


            var order =
                await _orderRepository.GetByIdAsync(
                    request.Id,
                    cancellationToken);


            if (order is null)
            {
                _logger.LogWarning(
                    "Order {OrderId} was not found",
                    request.Id);

                return Result<GetOrderResponse>.Failure(
                    "سفارش یافت نشد.");
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



            _logger.LogInformation(
                "Order {OrderId} retrieved successfully. " +
                "CustomerId: {CustomerId}, " +
                "Status: {Status}, " +
                "ItemsCount: {ItemsCount}",
                order.Id,
                order.CustomerId,
                order.Status,
                order.Items.Count);


            return Result<GetOrderResponse>.Success(
                response);
        }
    }
}
