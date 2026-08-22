using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Customers.GetCustomer
{
    public sealed class GetCustomerUseCase : IGetCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        public GetCustomerUseCase(
            ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<Result<GetCustomerResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var customer =
                await _customerRepository.GetByIdAsync(
                    id,
                    cancellationToken);

            if (customer is null)
            {
                return Result<GetCustomerResponse>.Failure(
                    "مشتری پیدا نشد.");
            }

            var response = new GetCustomerResponse(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PhoneNumber,
                customer.CreatedAt);

            return Result<GetCustomerResponse>.Success(
                response);
        }
    }
}
