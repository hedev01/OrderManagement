
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Features.Authentication.CreateUser;
using OrderManagement.Application.Features.Authentication.Login;
using OrderManagement.Application.Features.Customers.CreateCustomer;
using OrderManagement.Application.Features.Customers.GetCustomer;
using OrderManagement.Application.Features.Customers.GetCustomers;
using OrderManagement.Application.Features.Orders.CreateOrder;
using OrderManagement.Application.Features.Orders.GetOrder;
using OrderManagement.Application.Features.Orders.GetOrders;

namespace OrderManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<ICreateCustomerUseCase, CreateCustomerUseCase>();
            services.AddScoped<IGetCustomerUseCase, GetCustomerUseCase>();
            services.AddScoped<IGetCustomersUseCase, GetCustomersUseCase>();
            services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
            services.AddScoped<IGetOrderUseCase, GetOrderUseCase>();
            services.AddScoped<IGetOrdersUseCase, GetOrdersUseCase>();
            return services;
        }
    }
}
