using JobBoards.Data.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JobBoards.Data.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // User settings.
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedEmail = false;

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<JobBoardsDbContext>();

        services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();

        return services;
    }
}