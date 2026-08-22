using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Features.Customers.GetCustomer
{
    public interface IGetCustomerUseCase
    {
        Task<Result<GetCustomerResponse>> ExecuteAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
