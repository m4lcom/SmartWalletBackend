using Contracts.Requests;
using SmartWallet.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace SmartWallet.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.AuthenticateAsync(request);
        if (token == null) return Unauthorized(new { error = "Credenciales invalidas." });
        return Ok(new { token });
    }
}
