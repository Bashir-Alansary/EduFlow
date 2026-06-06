using Microsoft.AspNetCore.Identity;

namespace EduFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
