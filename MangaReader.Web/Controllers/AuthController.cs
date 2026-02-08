using MangaReader.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace MangaReader.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public AuthController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _loginUseCase.Execute(
            request.UserName,
            request.Password
        );

        return Ok(new { token });
    }
}

public record LoginRequest(string UserName, string Password);
