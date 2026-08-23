using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Orders.CreateOrder
{
    public sealed record CreateOrderRequest(
        Guid CustomerId,
        IReadOnlyList<CreateOrderItemRequest> Items);

    public sealed record CreateOrderItemRequest(
        Guid ProductId,
        [Range(1, int.MaxValue)]
        int Quantity);
}
