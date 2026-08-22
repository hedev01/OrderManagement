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
    public sealed class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<CreateUserResponse>> ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var exists =
                await _userRepository.ExistsByUsernameAsync(
                    request.Username,
                    cancellationToken);

            if (exists)
            {
                return Result<CreateUserResponse>.Failure("Username already exists.");
            }

            var passwordHash =
                _passwordHasher.Hash(request.Password);

            var user = new User(
                request.Username,
                passwordHash,
                request.Role);

            await _userRepository.AddAsync(
                user,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var response = new CreateUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString()
            };
            return Result<CreateUserResponse>.Success(response);
        }
    }
}
