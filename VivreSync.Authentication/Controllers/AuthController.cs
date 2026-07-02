using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.Authentication.DTOs;
using VivreSync.Authentication.Services;
using VivreSync.Shared.Exceptions;

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
        if(dto == null) throw new BadRequestException("Enter the Details");

        var result = _authService.Login(dto);
        if (result == null)
            throw new BadRequestException("Invalid UserName or Password");

        return Ok(result);
    }

    [HttpPost("ChangePassword")]
    [Authorize]
    public IActionResult ChangePassword(ChangePasswordDTO dto)
    {
        if (dto == null)
            throw new BadRequestException("Enter required data");

        var userId = GetCurrentUserId();
        var result = _authService.ChangePassword(userId, dto);
        if (!result)
            throw new BadRequestException("Cannot Change the Password");

        return Ok("Password changed successfully. Please login again.");
    }

    private int GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var currentUserId))
            throw new UnauthorizedException("Invalid token");

        return currentUserId;
    }
}