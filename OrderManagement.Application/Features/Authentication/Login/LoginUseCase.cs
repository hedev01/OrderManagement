using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Authentication;
using OrderManagement.Application.Interfaces.Persistence;

namespace OrderManagement.Application.Features.Authentication.Login
{
    public sealed class LoginUseCase : ILoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<Result<LoginResponse>> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user =
                await _userRepository.GetByUsernameAsync(
                    request.Username,
                    cancellationToken);

            if (user is null)
            {
                return Result<LoginResponse>.Failure("Invalid username or password.");
            }

            var passwordValid =
                _passwordHasher.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return Result<LoginResponse>.Failure("Invalid username or password.");
            }

            var token =
                _jwtTokenService.GenerateToken(user);

            var response = new LoginResponse
            {
                AccessToken = token,

            };
            return Result<LoginResponse>.Success(response);
        }
    }
}
