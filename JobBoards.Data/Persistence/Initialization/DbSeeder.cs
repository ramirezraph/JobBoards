using JobBoards.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace JobBoards.Data.Persistence.Initialization;

public static class DbSeeder
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = new[] { "Admin", "Employer", "User" };

        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@pjli.com",
            Email = "admin@pjli.com",
            FullName = "Administrator",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Pa$$w0rd!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            throw new InvalidOperationException("Failed to generate admin user.");
        }
    }
}