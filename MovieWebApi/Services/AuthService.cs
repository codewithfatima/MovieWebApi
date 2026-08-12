using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieWebApi.Dtos;
using MovieWebApi.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using MovieWebApi.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MovieWebApi.Services
{
    public class AuthService:IAuthService
    {
     private readonly UserManager<ApplicationUser> _userManager;
            private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager ,IConfiguration config )
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
                UserName = dto.Email,
            }; 

           var result = await _userManager.CreateAsync(user , dto.Password);
            return result;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var loginUser = await _userManager.FindByEmailAsync(dto.Email);

            if (loginUser == null)
            {
                return null;
            }

            var isPasswordVaild = await _userManager.CheckPasswordAsync(loginUser , dto.Password);
            if(!isPasswordVaild )
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(loginUser);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginUser.Id),
                new Claim(ClaimTypes.Email , loginUser.Email),

            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
    issuer: _config["Jwt:Issuer"],
    audience: _config["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(30),
    signingCredentials: creds
);
            return new JwtSecurityTokenHandler().WriteToken(token);
            //return "temporary-placeholder";
        }

    }
}