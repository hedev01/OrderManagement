using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Customers.GetCustomers
{
    public interface IGetCustomersUseCase
    {
        Task<Result<IReadOnlyList<GetCustomersResponse>>> ExecuteAsync(
            GetCustomersRequest request,
            CancellationToken cancellationToken);
    }
}
