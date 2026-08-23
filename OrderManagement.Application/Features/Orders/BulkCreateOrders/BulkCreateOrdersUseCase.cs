using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderManagement.Application.Features.Orders.BulkCreateOrders
{
    using Microsoft.Extensions.Logging;

    public sealed class BulkCreateOrdersUseCase
        : IBulkCreateOrdersUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BulkCreateOrdersUseCase> _logger;

        public BulkCreateOrdersUseCase(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ILogger<BulkCreateOrdersUseCase> logger)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<BulkCreateOrdersResponse>> ExecuteAsync(
            BulkCreateOrdersRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting bulk order creation. RequestedOrders: {OrdersCount}",
                request.Orders?.Count ?? 0);



            if (request.Orders is null ||
                request.Orders.Count == 0)
            {
                _logger.LogWarning(
                    "Bulk order creation failed. No orders were provided.");

                return Result<BulkCreateOrdersResponse>.Failure(
                    "حداقل یک سفارش الزامی است.");
            }


            if (request.Orders.Count > 1000)
            {
                _logger.LogWarning(
                    "Bulk order creation failed. " +
                    "RequestedOrders: {OrdersCount}, MaximumAllowed: 1000",
                    request.Orders.Count);

                return Result<BulkCreateOrdersResponse>.Failure(
                    "حداکثر ۱۰۰۰ سفارش برای هر درخواست مجاز است.");
            }


            foreach (var orderRequest in request.Orders)
            {
                if (orderRequest.Items is null ||
                    orderRequest.Items.Count == 0)
                {
                    _logger.LogWarning(
                        "Bulk order creation failed. " +
                        "An order does not contain any items.");

                    return Result<BulkCreateOrdersResponse>.Failure(
                        "هر سفارش باید حداقل شامل یک کالا باشد.");
                }


                if (orderRequest.Items.Any(x => x.Quantity <= 0))
                {
                    _logger.LogWarning(
                        "Bulk order creation failed. " +
                        "An order contains invalid quantity.");

                    return Result<BulkCreateOrdersResponse>.Failure(
                        "مقدار باید بزرگتر از صفر باشد.");
                }
            }



            var customerIds =
                request.Orders
                    .Select(x => x.CustomerId)
                    .Distinct()
                    .ToList();

            var productIds =
                request.Orders
                    .SelectMany(x => x.Items)
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();


            _logger.LogInformation(
                "Bulk order validation passed. " +
                "Orders: {OrdersCount}, " +
                "Customers: {CustomersCount}, " +
                "Products: {ProductsCount}",
                request.Orders.Count,
                customerIds.Count,
                productIds.Count);



            var customers =
                await _customerRepository.GetByIdsAsync(
                    customerIds,
                    cancellationToken);

            if (customers.Count != customerIds.Count)
            {
                _logger.LogWarning(
                    "Bulk order creation failed. " +
                    "Some customers were not found. " +
                    "RequestedCustomers: {RequestedCustomers}, " +
                    "FoundCustomers: {FoundCustomers}",
                    customerIds.Count,
                    customers.Count);

                return Result<BulkCreateOrdersResponse>.Failure(
                    "یک یا چند مشتری پیدا نشد.");
            }



            var products =
                await _productRepository.GetByIdsAsync(
                    productIds,
                    cancellationToken);

            if (products.Count != productIds.Count)
            {
                _logger.LogWarning(
                    "Bulk order creation failed. " +
                    "Some products were not found. " +
                    "RequestedProducts: {RequestedProducts}, " +
                    "FoundProducts: {FoundProducts}",
                    productIds.Count,
                    products.Count);

                return Result<BulkCreateOrdersResponse>.Failure(
                    "یک یا چند محصول یافت نشد.");
            }


            var productDictionary =
                products.ToDictionary(x => x.Id);



            var requestedQuantities =
                request.Orders
                    .SelectMany(x => x.Items)
                    .GroupBy(x => x.ProductId)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Sum(item => item.Quantity));


            _logger.LogInformation(
                "Inventory validation started for {ProductsCount} products.",
                requestedQuantities.Count);



            foreach (var requested in requestedQuantities)
            {
                var product =
                    productDictionary[requested.Key];


                if (product.Inventory is null)
                {
                    _logger.LogWarning(
                        "Bulk order creation failed. " +
                        "Inventory not found for Product {ProductId}.",
                        product.Id);

                    return Result<BulkCreateOrdersResponse>.Failure(
                        $"موجودی برای محصول {product.Id} پیدا نشد.");
                }


                if (product.Inventory.Quantity <
                    requested.Value)
                {
                    _logger.LogWarning(
                        "Bulk order creation failed due to insufficient inventory. " +
                        "ProductId: {ProductId}, " +
                        "ProductName: {ProductName}, " +
                        "AvailableQuantity: {AvailableQuantity}, " +
                        "RequestedQuantity: {RequestedQuantity}",
                        product.Id,
                        product.Name,
                        product.Inventory.Quantity,
                        requested.Value);

                    return Result<BulkCreateOrdersResponse>.Failure(
                        $"موجودی محصول {product.Name} کافی نیست. " +
                        $"موجودی فعلی: {product.Inventory.Quantity}، " +
                        $"مقدار درخواست شده: {requested.Value}.");
                }
            }


            _logger.LogInformation(
                "Inventory validation passed for bulk order creation. " +
                "Products: {ProductsCount}",
                requestedQuantities.Count);



            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);

            try
            {
                var orders =
                    new List<Order>(
                        request.Orders.Count);



                foreach (var orderRequest in request.Orders)
                {
                    var order =
                        new Order(
                            orderRequest.CustomerId);


                    foreach (var itemRequest in orderRequest.Items)
                    {
                        var product =
                            productDictionary[
                                itemRequest.ProductId];


                        var orderItem =
                            new OrderItem(
                                product.Id,
                                itemRequest.Quantity,
                                product.Price);


                        order.AddItem(orderItem);
                    }


                    orders.Add(order);
                }



                await _orderRepository.AddRangeAsync(
                    orders,
                    cancellationToken);


                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);



                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);


                _logger.LogInformation(
                    "Bulk order creation completed successfully. " +
                    "CreatedOrders: {CreatedOrders}",
                    orders.Count);



                var response =
                    new BulkCreateOrdersResponse(
                        orders.Count,
                        orders
                            .Select(x => x.Id)
                            .ToList());


                return Result<BulkCreateOrdersResponse>.Success(
                    response);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error occurred during bulk order creation. " +
                    "RequestedOrders: {OrdersCount}. " +
                    "Transaction will be rolled back.",
                    request.Orders.Count);


                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}
