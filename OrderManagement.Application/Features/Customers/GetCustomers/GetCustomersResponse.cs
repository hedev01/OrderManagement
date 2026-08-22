using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.GetCustomers
{
    public sealed record GetCustomersResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        DateTime CreatedAt);
}
