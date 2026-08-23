using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Customers.GetCustomer
{
    using Microsoft.Extensions.Logging;

    public sealed class GetCustomerUseCase
        : IGetCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomerUseCase> _logger;

        public GetCustomerUseCase(
            ICustomerRepository customerRepository,
            ILogger<GetCustomerUseCase> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<Result<GetCustomerResponse>> ExecuteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting customer {CustomerId}",
                id);


            var customer =
                await _customerRepository.GetByIdAsync(
                    id,
                    cancellationToken);


            if (customer is null)
            {
                _logger.LogWarning(
                    "Customer {CustomerId} was not found",
                    id);

                return Result<GetCustomerResponse>.Failure(
                    "مشتری پیدا نشد.");
            }


            var response =
                new GetCustomerResponse(
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.PhoneNumber,
                    customer.CreatedAt);



            _logger.LogInformation(
                "Customer {CustomerId} retrieved successfully",
                customer.Id);


            return Result<GetCustomerResponse>.Success(
                response);
        }
    }
}
