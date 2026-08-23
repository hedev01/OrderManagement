using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Orders.BulkCreateOrders;
using OrderManagement.Application.Features.Orders.ChangeOrderStatus;
using OrderManagement.Application.Features.Orders.CreateOrder;
using OrderManagement.Application.Features.Orders.DeleteOrder;
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
        private readonly IChangeOrderStatusUseCase _changeOrderStatusUseCase;
        private readonly IDeleteOrderUseCase _deleteOrderUseCase;
        private readonly IBulkCreateOrdersUseCase _bulkCreateOrdersUseCase;

        public OrdersController(
            ICreateOrderUseCase createOrderUseCase ,
            IGetOrderUseCase getOrderUseCase,
            IGetOrdersUseCase getOrdersUseCase ,
            IChangeOrderStatusUseCase changeOrderStatusUseCase ,
            IDeleteOrderUseCase deleteOrderUseCase ,
            IBulkCreateOrdersUseCase bulkCreateOrdersUseCase)
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrderUseCase = getOrderUseCase;
            _getOrdersUseCase = getOrdersUseCase;
            _changeOrderStatusUseCase = changeOrderStatusUseCase;
            _deleteOrderUseCase = deleteOrderUseCase;
            _bulkCreateOrdersUseCase = bulkCreateOrdersUseCase;
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

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(
            Guid id,
            [FromBody] ChangeOrderStatusRequest request,
            CancellationToken cancellationToken)
        {
            var command =
                request with
                {
                    OrderId = id
                };

            var result =
                await _changeOrderStatusUseCase.ExecuteAsync(
                    command,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(
            Guid id,
            CancellationToken cancellationToken)
        {
            var request =
                new DeleteOrderRequest(id);

            var result =
                await _deleteOrderUseCase.ExecuteAsync(
                    request,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return NoContent();
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateOrders(
            [FromBody] BulkCreateOrdersRequest request,
            CancellationToken cancellationToken)
        {
            var result =
                await _bulkCreateOrdersUseCase.ExecuteAsync(
                    request,
                    cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }
    }
}
