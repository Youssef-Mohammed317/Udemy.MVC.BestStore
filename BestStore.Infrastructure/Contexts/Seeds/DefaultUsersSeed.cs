using BestStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BestStore.Infrastructure.Contexts.Seeds
{
    public static class DefaultUsersSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var superAdminEmail = "superadmin@system.com";

            if (await userManager.FindByEmailAsync(superAdminEmail) == null)
            {
                var superAdmin = new ApplicationUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    Address = superAdminEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                };

                await userManager.CreateAsync(superAdmin, "Test@123");
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }

            var adminEmail = "admin@system.com";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = adminEmail,
                    Email = adminEmail,
                    Address = adminEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                };

                await userManager.CreateAsync(admin, "Test@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            var customerEmail = "customer@system.com";

            if (await userManager.FindByEmailAsync(customerEmail) == null)
            {
                var user = new ApplicationUser
                {
                    FirstName = "Customer",
                    LastName = "Customer",
                    UserName = customerEmail,
                    Email = customerEmail,
                    Address = customerEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                };

                await userManager.CreateAsync(user, "Test@123");
                await userManager.AddToRoleAsync(user, "Customer");
            }
        }
    }
}
