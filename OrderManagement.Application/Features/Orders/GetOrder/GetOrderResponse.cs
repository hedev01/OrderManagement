using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.GetOrder
{
    public sealed record GetOrderResponse(
        Guid Id,
        Guid CustomerId,
        OrderStatus Status,
        decimal TotalPrice,
        DateTime CreatedAt,
        IReadOnlyList<GetOrderItemResponse> Items);

    public sealed record GetOrderItemResponse(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice);
}
