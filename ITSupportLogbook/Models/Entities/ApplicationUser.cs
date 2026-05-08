using Microsoft.AspNetCore.Identity;

namespace ITSupportLogbook.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "users"; //admin, it, user


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string FullName => $"{FirstName} {LastName}";

    }
}
