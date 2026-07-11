using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace CollegeControlSystem.Infrastructure.Seeders
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
            var adminOptions = serviceProvider.GetRequiredService<IOptionsMonitor<AdminAccount>>();

            var adminAccount = adminOptions.CurrentValue;

            // Ensure Role exists
            string adminRole = Roles.AdminRole;
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = adminRole
                });
            }

            // Create admin user if not exists
            string adminEmail = adminAccount.Email;
            string adminPassword = adminAccount.Password;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminAccount.Password,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = adminAccount.PhoneNumber,
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }

    }

}
