using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaReader.Application.UseCases;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly LoginUseCase _loginUseCase;

    public AuthController(RegisterUserUseCase registerUserUseCase, LoginUseCase loginUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var token = await _registerUserUseCase.Execute(request.UserName, request.Password);
        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _loginUseCase.Execute(request.UserName, request.Password);
            return Ok(new { Token = token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }
    }
}

public record RegisterRequest(string UserName, string Password);
public record LoginRequest(string UserName, string Password);