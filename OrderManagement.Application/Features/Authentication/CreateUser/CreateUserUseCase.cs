using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Authentication;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Authentication.CreateUser
{
    using Microsoft.Extensions.Logging;

    public sealed class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateUserUseCase> _logger;

        public CreateUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            ILogger<CreateUserUseCase> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CreateUserResponse>> ExecuteAsync(
            CreateUserRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Creating user {Username} with role {Role}",
                request.Username,
                request.Role);


            var exists =
                await _userRepository.ExistsByUsernameAsync(
                    request.Username,
                    cancellationToken);

            if (exists)
            {
                _logger.LogWarning(
                    "Create user failed. Username {Username} already exists.",
                    request.Username);

                return Result<CreateUserResponse>.Failure(
                    "نام کاربری از قبل وجود دارد.");
            }



            var passwordHash =
                _passwordHasher.Hash(
                    request.Password);



            var user =
                new User(
                    request.Username,
                    passwordHash,
                    request.Role);


            await _userRepository.AddAsync(
                user,
                cancellationToken);



            await _unitOfWork.SaveChangesAsync(
                cancellationToken);



            _logger.LogInformation(
                "User {Username} created successfully. " +
                "UserId: {UserId}, Role: {Role}",
                user.Username,
                user.Id,
                user.Role);


            var response =
                new CreateUserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role.ToString()
                };


            return Result<CreateUserResponse>.Success(
                response);
        }
    }
}
