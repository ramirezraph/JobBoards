using AutoMapper;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Repositories.JobCategories;

public class JobCategoriesRepository : IJobCategoriesRepository
{
    private readonly JobBoardsDbContext _dbContext;
    public JobCategoriesRepository(JobBoardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(JobCategory entity)
    {
        await _dbContext.JobCategories.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
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
        _dbContext.JobCategories.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, JobCategory entity)
    {
        var jobCategory = await _dbContext.JobCategories.FindAsync(id);
        if (jobCategory is null)
        {
            throw new Exception("Trying to update job category that doesn't exists.");
        }

        jobCategory.Name = entity.Name;
        jobCategory.Description = entity.Description;
        jobCategory.UpdatedAt = DateTime.Now;

        _dbContext.JobCategories.Update(jobCategory);

        await _dbContext.SaveChangesAsync();
    }
}