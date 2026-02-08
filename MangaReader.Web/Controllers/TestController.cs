using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MangaReader.Web.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        return Ok(new { UserId = userId, Roles = roles });
    }
}
