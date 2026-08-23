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
    using Microsoft.Extensions.Logging;

    public sealed class LoginUseCase : ILoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<LoginUseCase> _logger;

        public LoginUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            ILogger<LoginUseCase> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> ExecuteAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Login attempt for user {Username}",
                request.Username);



            var user =
                await _userRepository.GetByUsernameAsync(
                    request.Username,
                    cancellationToken);



            if (user is null)
            {
                _logger.LogWarning(
                    "Login failed for username {Username}. " +
                    "User was not found.",
                    request.Username);

                return Result<LoginResponse>.Failure(
                    "نام کاربری یا رمز عبور نامعتبر است.");
            }



            var passwordValid =
                _passwordHasher.Verify(
                    request.Password,
                    user.PasswordHash);


            if (!passwordValid)
            {
                _logger.LogWarning(
                    "Login failed for username {Username}. " +
                    "Invalid password.",
                    request.Username);

                return Result<LoginResponse>.Failure(
                    "نام کاربری یا رمز عبور نامعتبر است.");
            }



            var token =
                _jwtTokenService.GenerateToken(
                    user);



            _logger.LogInformation(
                "User {Username} logged in successfully. " +
                "UserId: {UserId}, Role: {Role}",
                user.Username,
                user.Id,
                user.Role);


            var response =
                new LoginResponse
                {
                    AccessToken = token
                };


            return Result<LoginResponse>.Success(
                response);
        }
    }
}
