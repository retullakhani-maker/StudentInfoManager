using Microsoft.AspNetCore.Identity;

namespace Core.Model
{
    public class ApplicationUser : IdentityUser
    {
        public Students StudentProfile { get; set; }
        public string Role { get; set; }
        public string? FullName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
