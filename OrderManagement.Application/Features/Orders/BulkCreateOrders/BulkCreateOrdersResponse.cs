using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Orders.BulkCreateOrders
{
    public sealed record BulkCreateOrdersResponse(
        int CreatedCount,
        IReadOnlyList<Guid> OrderIds);
}
