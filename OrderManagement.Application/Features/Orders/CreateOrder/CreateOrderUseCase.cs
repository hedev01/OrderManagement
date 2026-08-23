using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderManagement.Application.Features.Orders.CreateOrder
{


    public sealed class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrderUseCase> _logger;

        public CreateOrderUseCase(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CreateOrderResponse>> ExecuteAsync(
            CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Creating order for customer {CustomerId}. ItemsCount: {ItemsCount}",
                request.CustomerId,
                request.Items?.Count ?? 0);



            if (request.Items is null ||
                request.Items.Count == 0)
            {
                _logger.LogWarning(
                    "Create order failed. Customer {CustomerId} provided an empty order.",
                    request.CustomerId);

                return Result<CreateOrderResponse>.Failure(
                    "سفارش باید حداقل شامل یک کالا باشد.");
            }


            if (request.Items.Any(x => x.Quantity <= 0))
            {
                _logger.LogWarning(
                    "Create order failed. Customer {CustomerId} provided invalid quantity.",
                    request.CustomerId);

                return Result<CreateOrderResponse>.Failure(
                    "مقدار باید بزرگتر از صفر باشد.");
            }



            var customer =
                await _customerRepository.GetByIdAsync(
                    request.CustomerId,
                    cancellationToken);

            if (customer is null)
            {
                _logger.LogWarning(
                    "Create order failed. Customer {CustomerId} was not found.",
                    request.CustomerId);

                return Result<CreateOrderResponse>.Failure(
                    "مشتری پیدا نشد.");
            }



            var productIds =
                request.Items
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

            var products =
                await _productRepository.GetByIdsAsync(
                    productIds,
                    cancellationToken);

            if (products.Count != productIds.Count)
            {
                _logger.LogWarning(
                    "Create order failed. One or more products were not found. " +
                    "CustomerId: {CustomerId}, RequestedProducts: {RequestedProducts}, " +
                    "FoundProducts: {FoundProducts}",
                    request.CustomerId,
                    productIds.Count,
                    products.Count);

                return Result<CreateOrderResponse>.Failure(
                    "یک یا چند محصول یافت نشد.");
            }


            var productDictionary =
                products.ToDictionary(x => x.Id);



            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var order =
                    new Order(request.CustomerId);



                foreach (var itemRequest in request.Items)
                {
                    var product =
                        productDictionary[
                            itemRequest.ProductId];




                    if (product.Inventory is null)
                    {
                        _logger.LogWarning(
                            "Create order failed. Inventory not found for Product {ProductId}. " +
                            "CustomerId: {CustomerId}",
                            product.Id,
                            request.CustomerId);

                        return Result<CreateOrderResponse>.Failure(
                            $"موجودی برای محصول یافت نشد {product.Id}.");
                    }




                    if (product.Inventory.Quantity <
                        itemRequest.Quantity)
                    {
                        _logger.LogWarning(
                            "Create order failed due to insufficient inventory. " +
                            "ProductId: {ProductId}, ProductName: {ProductName}, " +
                            "AvailableQuantity: {AvailableQuantity}, RequestedQuantity: {RequestedQuantity}, " +
                            "CustomerId: {CustomerId}",
                            product.Id,
                            product.Name,
                            product.Inventory.Quantity,
                            itemRequest.Quantity,
                            request.CustomerId);

                        return Result<CreateOrderResponse>.Failure(
                            $"موجودی کافی برای محصول وجود ندارد {product.Name}.");
                    }


                    var orderItem =
                        new OrderItem(
                            product.Id,
                            itemRequest.Quantity,
                            product.Price);

                    order.AddItem(orderItem);

                }



                await _orderRepository.AddAsync(
                    order,
                    cancellationToken);



                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);



                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);



                _logger.LogInformation(
                    "Order {OrderId} created successfully. " +
                    "CustomerId: {CustomerId}, " +
                    "Status: {Status}, " +
                    "ItemsCount: {ItemsCount}, " +
                    "TotalPrice: {TotalPrice}",
                    order.Id,
                    order.CustomerId,
                    order.Status,
                    order.Items.Count,
                    order.TotalPrice);



                var response =
                    new CreateOrderResponse(
                        order.Id,
                        order.CustomerId,
                        order.Status,
                        order.TotalPrice,
                        order.CreatedAt,
                        order.Items
                            .Select(item =>
                                new CreateOrderItemResponse(
                                    item.ProductId,
                                    item.Quantity,
                                    item.UnitPrice,
                                    item.TotalPrice))
                            .ToList());


                return Result<CreateOrderResponse>.Success(
                    response);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error occurred while creating order for Customer {CustomerId}. " +
                    "Transaction will be rolled back.",
                    request.CustomerId);

                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}
