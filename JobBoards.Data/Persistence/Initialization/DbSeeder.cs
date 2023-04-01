using JobBoards.Data.Entities;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Context;
using Microsoft.AspNetCore.Identity;

namespace JobBoards.Data.Persistence.Initialization;

public static class DbSeeder
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        if (roleManager.Roles.Any())
        {
            return;
        }

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
        if (userManager.Users.Any())
        {
            return;
        }

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

    public static async Task SeedJobCategories(JobBoardsDbContext dbContext)
    {
        if (dbContext.JobCategories.Any())
        {
            return;
        }

        var jobCategories = new List<JobCategory>
        {
            JobCategory.CreateNew("Admin / Human Resource", null),
            JobCategory.CreateNew("Client Relations", null),
            JobCategory.CreateNew("Engineering", null),
            JobCategory.CreateNew("Executive Positions", null),
            JobCategory.CreateNew("Finance", null),
            JobCategory.CreateNew("General Services", null),
            JobCategory.CreateNew("Information Technology", null),
            JobCategory.CreateNew("Operations", null),
            JobCategory.CreateNew("Sales and Marketing", null)
        };

        await dbContext.JobCategories.AddRangeAsync(jobCategories);
        await dbContext.SaveChangesAsync();
    }

    public static async Task SeedJobLocations(JobBoardsDbContext dbContext)
    {
        if (dbContext.JobLocations.Any())
        {
            return;
        }

        var jobLocations = new List<JobLocation>
        {
            JobLocation.CreateNew("Quezon City", "Philippines"),
            JobLocation.CreateNew("Makati", "Philippines"),
            JobLocation.CreateNew("Imus, Cavite", "Philippines"),
            JobLocation.CreateNew("Marikina", "Philippines"),
            JobLocation.CreateNew("Navotas", "Philippines"),
            JobLocation.CreateNew("Manila", "Philippines"),
            JobLocation.CreateNew("Mandaluyong", "Philippines"),
            JobLocation.CreateNew("Meycauayan, Bulacan", "Philippines"),
            JobLocation.CreateNew("Paranaque", "Philippines"),
            JobLocation.CreateNew("Taguig", "Philippines"),
            JobLocation.CreateNew("Bulakan, Bulacan", "Philippines"),
            JobLocation.CreateNew("Angeles, Pampanga", "Philippines"),
            JobLocation.CreateNew("Las Pinas", "Philippines"),
            JobLocation.CreateNew("Mariveles, Bataan", "Philippines"),
            JobLocation.CreateNew("Calamba, Laguna", "Philippines"),
            JobLocation.CreateNew("Batangas", "Philippines")
        };

        await dbContext.JobLocations.AddRangeAsync(jobLocations);
        await dbContext.SaveChangesAsync();
    }

    public static async Task SeedJobTypes(JobBoardsDbContext dbContext)
    {
        if (dbContext.JobTypes.Any())
        {
            return;
        }

        var jobTypes = new List<JobType>
        {
            JobType.CreateNew("Full-Time", null),
            JobType.CreateNew("Part-Time", null),
            JobType.CreateNew("Freelance", null),
        };

        await dbContext.JobTypes.AddRangeAsync(jobTypes);
        await dbContext.SaveChangesAsync();
    }
}