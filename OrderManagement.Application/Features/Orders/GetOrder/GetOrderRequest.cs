using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Orders.GetOrder
{
    public sealed record GetOrderRequest(Guid Id);
}
