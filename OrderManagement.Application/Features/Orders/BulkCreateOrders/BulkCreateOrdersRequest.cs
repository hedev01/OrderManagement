using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Features.Orders.CreateOrder;

namespace OrderManagement.Application.Features.Orders.BulkCreateOrders
{
    public sealed record BulkCreateOrdersRequest(
        IReadOnlyList<CreateOrderRequest> Orders);
}
