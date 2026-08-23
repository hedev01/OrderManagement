using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.GetOrders
{
    public sealed record GetOrdersResponse(
        Guid Id,
        Guid CustomerId,
        OrderStatus Status,
        decimal TotalPrice,
        DateTime CreatedAt);
}
