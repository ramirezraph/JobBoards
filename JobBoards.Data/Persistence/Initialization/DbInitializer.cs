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
    private readonly JobBoardsDbContext _dbContext;
    public DbInitializer(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<DbInitializer> logger,
        JobBoardsDbContext dbContext)
    {
        _roleManager = roleManager;
        _logger = logger;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task Initialize()
    {
        _logger.LogInformation("Initializing Database");

        await DbSeeder.SeedRoles(_roleManager);
        await DbSeeder.SeedAdminUser(_userManager);
        await DbSeeder.SeedJobCategories(_dbContext);
        await DbSeeder.SeedJobLocations(_dbContext);
        await DbSeeder.SeedJobTypes(_dbContext);
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
                if (context.Database.GetAppliedMigrations().Any())
                {
                    // Database exists and has migrations, ask if the user wants to drop the database.
                    Console.WriteLine("There are pending migrations. Do you want to drop the database and lose all data? (y/n)");
                    var response = Console.ReadLine();

                    if (response?.ToLower() == "y")
                    {
                        context.Database.EnsureDeleted();
                    }
                }

                await context.Database.MigrateAsync();
            }

            var dbInitializerService = services.GetRequiredService<IDbInitializer>();
            await dbInitializerService.Initialize();
        }
    }
}