using JobBoards.Data.Persistence.Context;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobBoards.Data.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException("ConnectionString cannot be null.");
        }

        services.AddDbContext<JobBoardsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IJobCategoriesRepository, JobCategoriesRepository>();

        return services;
    }
}