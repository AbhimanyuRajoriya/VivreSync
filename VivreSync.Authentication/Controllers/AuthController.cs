using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivreSync.Authentication.DTOs;
using VivreSync.Authentication.Services;

namespace VivreSync.Authentication.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login(LoginDTO dto)
    {
        var result = _authService.Login(dto);

        if (result == null)
            return BadRequest("Invalid username or password");

        return Ok(result);
    }

    [HttpPost("change-password")]
    [AllowAnonymous]
    public IActionResult ChangePassword(ChangePasswordDTO dto)
    {
        var result = _authService.ChangePassword(dto);

        if (!result)
            return BadRequest("Invalid username or old password");

        return Ok("Password changed successfully");
    }
}