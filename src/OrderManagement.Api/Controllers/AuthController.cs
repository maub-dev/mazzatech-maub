using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Contracts;
using OrderManagement.Application.Abstractions;

namespace OrderManagement.Api.Controllers;

[Route("auth")]
public sealed class AuthController(IAuthTokenService tokenService) : MainController
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        const string email = "dev@mazzatech.com";
        const string password = "Senha@123";
        if (!string.Equals(request.Email, email, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(request.Password, password, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        return Ok(new LoginResponse(
            tokenService.CreateToken(email),
            "Bearer",
            DateTime.UtcNow.AddHours(1)));
    }
}
