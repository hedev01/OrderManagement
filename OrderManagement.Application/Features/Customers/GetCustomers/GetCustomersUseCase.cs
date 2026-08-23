using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Customers.GetCustomers
{
    using Microsoft.Extensions.Logging;

    public sealed class GetCustomersUseCase
        : IGetCustomersUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomersUseCase> _logger;

        public GetCustomersUseCase(
            ICustomerRepository customerRepository,
            ILogger<GetCustomersUseCase> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<
                Result<IReadOnlyList<GetCustomersResponse>>>
            ExecuteAsync(
                GetCustomersRequest request,
                CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting all customers.");


            var customers =
                await _customerRepository.GetAllAsync(
                    cancellationToken);


            var response =
                customers
                    .Select(customer =>
                        new GetCustomersResponse(
                            customer.Id,
                            customer.FirstName,
                            customer.LastName,
                            customer.Email,
                            customer.PhoneNumber,
                            customer.CreatedAt))
                    .ToList();


            _logger.LogInformation(
                "Customers retrieved successfully. " +
                "ReturnedCount: {ReturnedCount}",
                response.Count);


            return Result<
                    IReadOnlyList<GetCustomersResponse>>
                .Success(response);
        }
    }
}
