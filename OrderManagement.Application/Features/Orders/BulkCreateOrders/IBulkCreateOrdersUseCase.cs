using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Orders.BulkCreateOrders
{
    public interface IBulkCreateOrdersUseCase
    {
        Task<Result<BulkCreateOrdersResponse>> ExecuteAsync(
            BulkCreateOrdersRequest request,
            CancellationToken cancellationToken);
    }
}
