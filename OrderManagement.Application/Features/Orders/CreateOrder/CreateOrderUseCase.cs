using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public CreateOrderUseCase(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateOrderResponse>> ExecuteAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
        {
            if (request.Items is null ||
                request.Items.Count == 0)
            {
                return Result<CreateOrderResponse>.Failure("سفارش باید حداقل شامل یک کالا باشد.");
            }

            if (request.Items.Any(x => x.Quantity <= 0))
            {
                return Result<CreateOrderResponse>.Failure("مقدار باید بزرگتر از صفر باشد.");
            }

            var customer =
                await _customerRepository.GetByIdAsync(
                    request.CustomerId,
                    cancellationToken);

            if (customer is null)
            {
                return Result<CreateOrderResponse>.Failure("مشتری پیدا نشد.");
            }

            var productIds = request.Items
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var products =
                await _productRepository.GetByIdsAsync(
                    productIds,
                    cancellationToken);

            if (products.Count != productIds.Count)
            {
                return Result<CreateOrderResponse>.Failure("یک یا چند محصول یافت نشد.");
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
                        productDictionary[itemRequest.ProductId];

                    if (product.Inventory is null)
                    {
                        return Result<CreateOrderResponse>.Failure($"موجودی برای محصول یافت نشد {product.Id}.");
                    }

                    if (product.Inventory.Quantity <
                        itemRequest.Quantity)
                    {
                        return Result<CreateOrderResponse>.Failure($"موجودی کافی برای محصول وجود ندارد {product.Name}.");
                    }

                    var orderItem =
                        new OrderItem(
                            product.Id,
                            itemRequest.Quantity,
                            product.Price);

                    order.AddItem(orderItem);

                    product.Inventory.Decrease(
                        itemRequest.Quantity);
                }

                await _orderRepository.AddAsync(
                    order,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);

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
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}
