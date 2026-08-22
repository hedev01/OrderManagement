using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.GetCustomer
{
    public sealed record GetCustomerResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        DateTime CreatedAt);
}
