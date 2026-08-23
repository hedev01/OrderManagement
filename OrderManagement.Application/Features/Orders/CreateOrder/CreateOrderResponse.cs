using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.CreateOrder
{
    public sealed record CreateOrderResponse(
        Guid Id,
        Guid CustomerId,
        OrderStatus Status,
        decimal TotalPrice,
        DateTime CreatedAt,
        IReadOnlyList<CreateOrderItemResponse> Items);

    public sealed record CreateOrderItemResponse(
        Guid ProductId,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice);
}
