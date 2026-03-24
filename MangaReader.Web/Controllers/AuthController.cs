using Microsoft.AspNetCore.Mvc;
using MangaReader.Application.UseCases;

namespace MangaReader.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly LoginUseCase _loginUseCase;

        public AuthController(
            RegisterUserUseCase registerUserUseCase,
            LoginUseCase loginUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _loginUseCase = loginUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var languageId = request.PreferredLanguageId ?? Guid.Empty;

                var token = await _registerUserUseCase.Execute(
                    request.UserName,
                    request.Password,
                    languageId);

                return Ok(new AuthResponse(token));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _loginUseCase.Execute(
                    request.UserName,
                    request.Password);

                return Ok(new AuthResponse(token));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public record RegisterRequest(
        string UserName,
        string Password,
        Guid? PreferredLanguageId
    );

    public record LoginRequest(
        string UserName,
        string Password
    );

    public record AuthResponse(string Token);
}