using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObrotoSklad.Application.Auth;

namespace ObrotoSklad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Created(string.Empty, response);
        }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            if (response is null)
            {
                return Unauthorized("Nieprawidłowy email lub hasło.");
            }
            return Ok(response);
        }
    }

    
}
