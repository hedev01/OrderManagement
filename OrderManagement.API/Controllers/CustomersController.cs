using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Customers.CreateCustomer;
using OrderManagement.Application.Features.Customers.GetCustomer;
using OrderManagement.Application.Features.Customers.GetCustomers;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICreateCustomerUseCase _createCustomerUseCase;
        private readonly IGetCustomerUseCase _getCustomerUseCase;
        private readonly IGetCustomersUseCase _getCustomersUseCase;

        public CustomersController(
            ICreateCustomerUseCase createCustomerUseCase,
            IGetCustomerUseCase getCustomerUseCase,
            IGetCustomersUseCase getCustomersUseCase)
        {
            _createCustomerUseCase = createCustomerUseCase;
            _getCustomerUseCase = getCustomerUseCase;
            _getCustomersUseCase = getCustomersUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CreateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            var result =
                await _createCustomerUseCase.ExecuteAsync(
                    request,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Created(
                $"/api/customers/{result.Data!.Id}",
                result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomer(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result =
                await _getCustomerUseCase.ExecuteAsync(
                    id,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers(
            CancellationToken cancellationToken)
        {
            var result =
                await _getCustomersUseCase.ExecuteAsync(
                    new GetCustomersRequest(),
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
