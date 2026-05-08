using ITSupportLogbook.Data;
using ITSupportLogbook.Models.DTOs;
using ITSupportLogbook.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITSupportLogbook.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
       // private readonly PasswordHasherService _passwordService;
       // private readonly UserNamingService _namingService;

        // 👇 CONSTRUCTOR 
        public AuthService(
            ApplicationDbContext context)
           // PasswordHasherService passwordService,
           // UserNamingService namingService)
        {
            _context = context;
           // _passwordService = passwordService;
           // _namingService = namingService;
        }

        public async Task<ApplicationUser> RegisterAsync(RegisterUserDto dto)
        {
            // ✅ use injected service (NOT static)
           // var username = _namingService.GenerateUsername(dto.Firstname, dto.Lastname);
            //var email = _namingService.GenerateEmail(dto.Firstname, dto.Lastname);

            // Check uniqueness
       
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = "User",
                PasswordHash = string.Empty, // will be set after hashing

            };

            //user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
