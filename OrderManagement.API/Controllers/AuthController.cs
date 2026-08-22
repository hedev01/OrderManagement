using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Features.Authentication.CreateUser;
using OrderManagement.Application.Features.Authentication.Login;

namespace OrderManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginUseCase _loginUseCase;
        private readonly ICreateUserUseCase _createUserUseCase;
        public AuthController(
            ILoginUseCase loginUseCase,
            ICreateUserUseCase createUserUseCase)
        {
            _loginUseCase = loginUseCase;
            _createUserUseCase = createUserUseCase;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _loginUseCase.ExecuteAsync(
                request,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpPost("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _createUserUseCase.ExecuteAsync(
                request,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Created(
                $"/api/auth/users/{result.Data!.Id}",
                result.Data);
        }
    }
}
