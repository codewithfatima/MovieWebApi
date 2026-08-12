using MovieWebApi.Dtos;
using System.ComponentModel.DataAnnotations;


namespace MovieWebApi.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
