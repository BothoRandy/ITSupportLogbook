using System;
using System.Threading.Tasks;
using ITSupportLogbook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ITSupportLogbook.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminRole = "Admin";
            const string adminUsername = "admin";
            const string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            var adminUser = await userManager.FindByNameAsync(adminUsername);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = "admin@local",
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors));
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
