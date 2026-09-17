using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> RefreshToken(
    [FromBody] RefreshTokenRequest request)
        {
            var response = await _authService
                .RefreshTokenAsync(request);

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
    [FromBody] RefreshTokenRequest request)
        {
            await _authService.LogoutAsync(request.RefreshToken);

            return Ok(new
            {
                message = "Logged out successfully."
            });
        }
    }
}
