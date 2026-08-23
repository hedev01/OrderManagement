using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Orders.DeleteOrder
{
    using Microsoft.Extensions.Logging;

    public sealed class DeleteOrderUseCase
        : IDeleteOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteOrderUseCase> _logger;

        public DeleteOrderUseCase(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> ExecuteAsync(
            DeleteOrderRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Deleting order {OrderId}",
                request.OrderId);


            var order =
                await _orderRepository.GetForDeleteAsync(
                    request.OrderId,
                    cancellationToken);


            if (order is null)
            {
                _logger.LogWarning(
                    "Delete failed. Order {OrderId} was not found",
                    request.OrderId);

                return Result<bool>.Failure(
                    "سفارشی یافت نشد.");
            }



            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {

                await _orderRepository.DeleteAsync(
                    order,
                    cancellationToken);



                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);



                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);



                _logger.LogInformation(
                    "Order {OrderId} deleted successfully. " +
                    "CustomerId: {CustomerId}",
                    order.Id,
                    order.CustomerId);


                return Result<bool>.Success(true);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error occurred while deleting order {OrderId}. " +
                    "Transaction will be rolled back.",
                    order.Id);

                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}
