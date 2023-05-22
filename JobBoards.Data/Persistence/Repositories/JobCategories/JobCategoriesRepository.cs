using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobBoards.Data.Persistence.Repositories.JobCategories;

public class JobCategoriesRepository : IJobCategoriesRepository
{
    private readonly JobBoardsDbContext _dbContext;
    private readonly ILogger<JobCategoriesRepository> _logger;
    public JobCategoriesRepository(JobBoardsDbContext dbContext, ILogger<JobCategoriesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddAsync(JobCategory entity)
    {
        try
        {
            var parameters = new[] {
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Description", string.IsNullOrEmpty(entity.Description) ? DBNull.Value : entity.Description),
                new SqlParameter("@CreatedAt", DateTime.Now)
            };
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC createJobCategory @Name, @Description, @CreatedAt", parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create job category. " + ex.ToString());
        }
    }

    public async Task<List<JobCategory>> GetAllAsync()
    {
        return await _dbContext.JobCategories
            .FromSql($"EXEC getJobCategories")
            .ToListAsync();
    }

    public async Task<JobCategory?> GetByIdAsync(Guid id)
    {
        var jobCategory = await _dbContext.JobCategories
            .FromSql($"EXEC getJobCategoryById {id}")
            .ToListAsync();

        return jobCategory.FirstOrDefault();
    }

    public async Task RemoveAsync(JobCategory entity)
    {
        await _dbContext.Database.ExecuteSqlAsync($"DELETE FROM JobCategories WHERE Id = {entity.Id}");
    }

    public async Task UpdateAsync(Guid id, JobCategory entity)
    {
        var jobCategory = await _dbContext.JobCategories.FindAsync(id);
        if (jobCategory is null)
        {
            throw new Exception("Trying to update job category that doesn't exists.");
        }

        try
        {
            var parameters = new[] {
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Description", string.IsNullOrEmpty(entity.Description) ? DBNull.Value : entity.Description),
                new SqlParameter("@UpdatedAt", DateTime.Now)
            };
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC updateJobCategory @Id, @Name, @Description, @UpdatedAt", parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update job category. " + ex.ToString());
        }
    }
}