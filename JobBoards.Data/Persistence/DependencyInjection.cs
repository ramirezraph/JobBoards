using JobBoards.Data.Persistence.Context;
using JobBoards.Data.Persistence.Repositories.JobCategories;
using JobBoards.Data.Persistence.Repositories.JobLocations;
using JobBoards.Data.Persistence.Repositories.JobTypes;
using JobBoards.Data.Persistence.Repositories.Resumes;
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

        services.AddScoped<IJobLocationsRepository, JobLocationsRepository>();

        services.AddScoped<IJobTypesRepository, JobTypesRepository>();

        services.AddScoped<IResumesRepository, ResumesRepository>();

        return services;
    }
}