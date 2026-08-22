using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Customers.GetCustomers
{
    public sealed class GetCustomersUseCase : IGetCustomersUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersUseCase(
            ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<Result<IReadOnlyList<GetCustomersResponse>>> ExecuteAsync(GetCustomersRequest request, CancellationToken cancellationToken)
        {
            var customers =
                await _customerRepository.GetAllAsync(
                    cancellationToken);

            var response = customers
                .Select(customer =>
                    new GetCustomersResponse(
                        customer.Id,
                        customer.FirstName,
                        customer.LastName,
                        customer.Email,
                        customer.PhoneNumber,
                        customer.CreatedAt))
                .ToList();

            return Result<IReadOnlyList<GetCustomersResponse>>
                .Success(response);
        }
    }
}
