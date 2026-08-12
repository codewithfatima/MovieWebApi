using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MovieWebApi.Dtos;

namespace MovieWebApi.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
