using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderManagement.Application.Features.Customers.CreateCustomer
{
    using Microsoft.Extensions.Logging;

    public sealed class CreateCustomerUseCase
        : ICreateCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCustomerUseCase> _logger;

        public CreateCustomerUseCase(
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateCustomerUseCase> logger)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CreateCustomerResponse>> ExecuteAsync(
            CreateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            var firstName =
                request.FirstName.Trim();

            var lastName =
                request.LastName.Trim();

            var email =
                request.Email.Trim().ToLowerInvariant();

            var phoneNumber =
                request.PhoneNumber.Trim();


            _logger.LogInformation(
                "Creating customer. Email: {Email}",
                email);



            var emailExists =
                await _customerRepository.ExistsByEmailAsync(
                    email,
                    cancellationToken);


            if (emailExists)
            {
                _logger.LogWarning(
                    "Create customer failed. " +
                    "Customer with email {Email} already exists.",
                    email);

                return Result<CreateCustomerResponse>.Failure(
                    "مشتری با این ایمیل از قبل وجود دارد.");
            }



            var customer =
                new Customer(
                    firstName,
                    lastName,
                    email,
                    phoneNumber);


            await _customerRepository.AddAsync(
                customer,
                cancellationToken);



            await _unitOfWork.SaveChangesAsync(
                cancellationToken);



            _logger.LogInformation(
                "Customer created successfully. " +
                "CustomerId: {CustomerId}",
                customer.Id);


            var response =
                new CreateCustomerResponse(
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.PhoneNumber,
                    customer.CreatedAt);


            return Result<CreateCustomerResponse>.Success(
                response);
        }
    }
}
