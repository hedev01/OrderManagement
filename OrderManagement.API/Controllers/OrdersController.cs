using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Orders.CreateOrder;
using OrderManagement.Application.Features.Orders.GetOrder;
using OrderManagement.Application.Features.Orders.GetOrders;

namespace OrderManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class OrdersController : ControllerBase
    {
        private readonly ICreateOrderUseCase _createOrderUseCase;
        private readonly IGetOrderUseCase _getOrderUseCase;
        private readonly IGetOrdersUseCase _getOrdersUseCase;

        public OrdersController(
            ICreateOrderUseCase createOrderUseCase , IGetOrderUseCase getOrderUseCase, IGetOrdersUseCase getOrdersUseCase)
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrderUseCase = getOrderUseCase;
            _getOrdersUseCase = getOrdersUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            var result =
                await _createOrderUseCase.ExecuteAsync(
                    request,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Created(
                $"/api/orders/{result.Data!.Id}",
                result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var request =
                new GetOrderRequest(id);

            var result =
                await _getOrderUseCase.ExecuteAsync(
                    request,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] GetOrdersRequest request,
            CancellationToken cancellationToken)
        {
            var result =
                await _getOrdersUseCase.ExecuteAsync(
                    request,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
