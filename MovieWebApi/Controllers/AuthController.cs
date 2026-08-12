using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;

namespace MovieWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
              _authService = authService; 
        }

        [HttpPost("register")]  
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            try
            {
                await _authService.RegisterAsync(dto);
                return Ok(new { message = "User registered successfully" });
            }catch(Exception ex)
            {
                return StatusCode(500, new {error = ex.Message , innerException = ex.InnerException?.Message });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token =  await _authService.LoginAsync(dto);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            return Ok(new {token});
        }

        
    }
}
