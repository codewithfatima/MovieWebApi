using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieWebApi.Data;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description)
                );

                throw new Exception($"Registration failed: {errors}");
            }

            return result;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            var passwordValid =
                await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? string.Empty
                )
            };

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, role)
                );
            }

            var jwtKey = _config["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("Jwt:Key is missing.");
            }

            var issuer = _config["Jwt:Issuer"];

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new Exception("Jwt:Issuer is missing.");
            }

            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new Exception("Jwt:Audience is missing.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}