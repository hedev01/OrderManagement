using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderManagement.Application.Features.Orders.ChangeOrderStatus
{
    using Microsoft.Extensions.Logging;

    public sealed class ChangeOrderStatusUseCase
        : IChangeOrderStatusUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChangeOrderStatusUseCase> _logger;

        public ChangeOrderStatusUseCase(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ILogger<ChangeOrderStatusUseCase> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<ChangeOrderStatusResponse>> ExecuteAsync(
            ChangeOrderStatusRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Changing status of order {OrderId} to {RequestedStatus}",
                request.OrderId,
                request.Status);



            var order =
                await _orderRepository.GetForStatusChangeAsync(
                    request.OrderId,
                    cancellationToken);



            if (order is null)
            {
                _logger.LogWarning(
                    "Change order status failed. " +
                    "Order {OrderId} was not found.",
                    request.OrderId);

                return Result<ChangeOrderStatusResponse>.Failure(
                    "سفارش یافت نشد.");
            }


            var previousStatus =
                order.Status;


            _logger.LogInformation(
                "Order {OrderId} current status is {PreviousStatus}. " +
                "Requested status: {RequestedStatus}",
                order.Id,
                previousStatus,
                request.Status);



            if (previousStatus == request.Status)
            {
                _logger.LogWarning(
                    "Order {OrderId} is already in status {Status}.",
                    order.Id,
                    request.Status);

                return Result<ChangeOrderStatusResponse>.Failure(
                    "سفارش از قبل در وضعیت درخواستی قرار دارد.");
            }



            if (request.Status == OrderStatus.Confirmed)
            {
                _logger.LogInformation(
                    "Validating inventory for order {OrderId}. " +
                    "ItemsCount: {ItemsCount}",
                    order.Id,
                    order.Items.Count);


                foreach (var item in order.Items)
                {
                    if (item.Product.Inventory is null)
                    {
                        _logger.LogWarning(
                            "Order {OrderId} confirmation failed. " +
                            "Inventory not found for Product {ProductId}.",
                            order.Id,
                            item.ProductId);

                        return Result<ChangeOrderStatusResponse>.Failure(
                            $"موجودی برای محصول یافت نشد {item.ProductId}.");
                    }


                    if (item.Product.Inventory.Quantity <
                        item.Quantity)
                    {
                        _logger.LogWarning(
                            "Order {OrderId} confirmation failed due to insufficient inventory. " +
                            "ProductId: {ProductId}, ProductName: {ProductName}, " +
                            "AvailableQuantity: {AvailableQuantity}, " +
                            "RequiredQuantity: {RequiredQuantity}",
                            order.Id,
                            item.ProductId,
                            item.Product.Name,
                            item.Product.Inventory.Quantity,
                            item.Quantity);

                        return Result<ChangeOrderStatusResponse>.Failure(
                            $"موجودی کافی برای محصول وجود ندارد {item.Product.Name}.");
                    }
                }

                _logger.LogInformation(
                    "Inventory validation passed for order {OrderId}.",
                    order.Id);
            }



            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {

                if (request.Status == OrderStatus.Confirmed)
                {
                    foreach (var item in order.Items)
                    {
                        _logger.LogInformation(
                            "Decreasing inventory for Product {ProductId}. " +
                            "Quantity: {Quantity}",
                            item.ProductId,
                            item.Quantity);

                        item.Product.Inventory!.Decrease(
                            item.Quantity);
                    }
                }



                order.ChangeStatus(
                    request.Status);



                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);



                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);



                _logger.LogInformation(
                    "Order {OrderId} status changed successfully. " +
                    "PreviousStatus: {PreviousStatus}, " +
                    "NewStatus: {NewStatus}",
                    order.Id,
                    previousStatus,
                    order.Status);


                var response =
                    new ChangeOrderStatusResponse(
                        order.Id,
                        previousStatus,
                        order.Status);


                return Result<ChangeOrderStatusResponse>.Success(
                    response);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error occurred while changing status of order {OrderId} " +
                    "from {PreviousStatus} to {RequestedStatus}. " +
                    "Transaction will be rolled back.",
                    order.Id,
                    previousStatus,
                    request.Status);

                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}
