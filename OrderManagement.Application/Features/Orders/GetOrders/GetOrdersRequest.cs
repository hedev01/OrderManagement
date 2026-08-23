using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.GetOrders
{
    public sealed record GetOrdersRequest(
        Guid? CustomerId,
        OrderStatus? Status,
        DateTime? FromDate,
        DateTime? ToDate,
        int Page = 1,
        int PageSize = 10);
}
