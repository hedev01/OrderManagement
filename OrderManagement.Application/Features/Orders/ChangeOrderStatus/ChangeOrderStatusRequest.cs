using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.ChangeOrderStatus
{
    public sealed record ChangeOrderStatusRequest(
        Guid OrderId,
        OrderStatus Status);
}
