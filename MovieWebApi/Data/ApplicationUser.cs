using Microsoft.AspNetCore.Identity;

namespace MovieWebApi.Data
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public int? DepartmentId { get; set; }
    }

}
