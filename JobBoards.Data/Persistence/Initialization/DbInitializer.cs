using JobBoards.Data.Identity;
using JobBoards.Data.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JobBoards.Data.Persistence.Initialization;

public class DbInitializer : IDbInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DbInitializer> _logger;
    public DbInitializer(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<DbInitializer> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Initialize()
    {
        _logger.LogInformation("Initializing Database");

        _logger.LogInformation("Adding Roles");
        await DbSeeder.SeedRoles(_roleManager);

        _logger.LogInformation("Adding Admin User");
        await DbSeeder.SeedAdminUser(_userManager);
    }

    public static async Task InitializeDatabase(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<JobBoardsDbContext>();

            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var dbInitializerService = services.GetRequiredService<IDbInitializer>();

                await dbInitializerService.Initialize();
            }
        }
    }
}