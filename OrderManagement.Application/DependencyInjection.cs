
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Features.Authentication.CreateUser;
using OrderManagement.Application.Features.Authentication.Login;

namespace OrderManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            return services;
        }
    }
}
