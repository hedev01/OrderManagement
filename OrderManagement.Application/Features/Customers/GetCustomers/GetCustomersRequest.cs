using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.GetCustomers
{
    public sealed record GetCustomersRequest(
        int Page = 1,
        int PageSize = 20);
}
