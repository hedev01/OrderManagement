using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Tests.Application.Orders
{
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrderManagement.Application.Features.Orders.CreateOrder;
    using OrderManagement.Application.Interfaces.Persistence;
    using Xunit;
    using OrderManagement.Application.Features.Orders.CreateOrder;
    using OrderManagement.Domain.Enums;

    public sealed class CreateOrderUseCaseTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateOrderUseCase>> _loggerMock;

        private readonly CreateOrderUseCase _useCase;

        public CreateOrderUseCaseTests()
        {
            _orderRepositoryMock =
                new Mock<IOrderRepository>();

            _customerRepositoryMock =
                new Mock<ICustomerRepository>();

            _productRepositoryMock =
                new Mock<IProductRepository>();

            _unitOfWorkMock =
                new Mock<IUnitOfWork>();

            _loggerMock =
                new Mock<ILogger<CreateOrderUseCase>>();

            _useCase =
                new CreateOrderUseCase(
                    _orderRepositoryMock.Object,
                    _customerRepositoryMock.Object,
                    _productRepositoryMock.Object,
                    _unitOfWorkMock.Object,
                    _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenItemsAreEmpty()
        {

            var request =
                new CreateOrderRequest(Guid.NewGuid(), []);



            var result =
                await _useCase.ExecuteAsync(
                    request,
                    CancellationToken.None);


            Assert.False(result.IsSuccess);

            Assert.Equal(
                "سفارش باید حداقل شامل یک کالا باشد.",
                result.ErrorMessage);
        }
        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenQuantityIsInvalid()
        {


            var request =
                new CreateOrderRequest(Guid.NewGuid(), [new CreateOrderItemRequest(Guid.NewGuid(), 0)]);


            var result =
                await _useCase.ExecuteAsync(
                    request,
                    CancellationToken.None);



            Assert.False(result.IsSuccess);

            Assert.Equal(
                "مقدار باید بزرگتر از صفر باشد.",
                result.ErrorMessage);
        }
        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenCustomerDoesNotExist()
        {

            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();


            var request =
                new CreateOrderRequest(customerId, [new CreateOrderItemRequest(productId, 2)]);


            _customerRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        customerId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);


            var result =
                await _useCase.ExecuteAsync(
                    request,
                    CancellationToken.None);



            Assert.False(result.IsSuccess);

            Assert.Equal(
                "مشتری پیدا نشد.",
                result.ErrorMessage);


            _productRepositoryMock.Verify(
                x => x.GetByIdsAsync(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenProductDoesNotExist()
        {

            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var customer =
                new Customer(
                    "Ali",
                    "Nazari",
                    "ali@test.com",
                    "09120000000");



            var request = new CreateOrderRequest(customerId, [new CreateOrderItemRequest(productId, 2)]);


            _customerRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        customerId,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);


            _productRepositoryMock
                .Setup(x =>
                    x.GetByIdsAsync(
                        It.IsAny<List<Guid>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());


            var result =
                await _useCase.ExecuteAsync(
                    request,
                    CancellationToken.None);


            Assert.False(result.IsSuccess);

            Assert.Equal(
                "یک یا چند محصول یافت نشد.",
                result.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCreateOrderSuccessfully()
        {

            var customer =
                new Customer(
                    "Ali",
                    "Nazari",
                    "ali@test.com",
                    "09120000000");

            var product =
                new Product(
                    "Laptop",
                    "Test Laptop",
                    540000);

            var request =
                new CreateOrderRequest(
                    customer.Id,
                    new List<CreateOrderItemRequest>
                    {
                new(
                    product.Id,
                    2)
                    });



            var inventory =
                new Inventory(
                    product.Id,
                    10);

            product.SetInventory(inventory);

            _customerRepositoryMock
                .Setup(x =>
                    x.GetByIdAsync(
                        customer.Id,
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _productRepositoryMock
                .Setup(x =>
                    x.GetByIdsAsync(
                        It.IsAny<List<Guid>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new List<Product>
                    {
                product
                    });

            _orderRepositoryMock
                .Setup(x =>
                    x.AddAsync(
                        It.IsAny<Order>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x =>
                    x.BeginTransactionAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x =>
                    x.SaveChangesAsync(
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _unitOfWorkMock
                .Setup(x =>
                    x.CommitTransactionAsync(
                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);




            var result =
                await _useCase.ExecuteAsync(
                    request,
                    CancellationToken.None);




            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            Assert.Equal(
                customer.Id,
                result.Data!.CustomerId);

            Assert.Equal(
                OrderStatus.Pending,
                result.Data.Status);

            Assert.Single(result.Data.Items);

            var item =
                result.Data.Items.First();

            Assert.Equal(
                product.Id,
                item.ProductId);

            Assert.Equal(
                2,
                item.Quantity);

            Assert.Equal(
                540000,
                item.UnitPrice);

            Assert.Equal(
                1080000,
                item.TotalPrice);


            _orderRepositoryMock.Verify(
                x =>
                    x.AddAsync(
                        It.IsAny<Order>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x =>
                    x.SaveChangesAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x =>
                    x.CommitTransactionAsync(
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

    }
}
