using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Repositories.JobPosts;

public class JobPostsRepository : IJobPostsRepository
{
    private readonly JobBoardsDbContext _dbContext;
    public JobPostsRepository(JobBoardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(JobPost entity)
    {
        await _dbContext.JobPosts.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<JobPost>> GetAllAsync()
    {
        return await _dbContext.JobPosts
            .Include(jp => jp.JobType)
            .Include(jp => jp.JobCategory)
            .Include(jp => jp.JobLocation)
            .Include(jp => jp.CreatedBy)
            .Include(jp => jp.JobApplications)
            .ToListAsync();
    }

    public IQueryable<JobPost> GetAllQueryable()
    {
        return _dbContext.JobPosts
            .Include(jp => jp.JobType)
            .Include(jp => jp.JobCategory)
            .Include(jp => jp.JobLocation)
            .Include(jp => jp.CreatedBy)
            .Include(jp => jp.JobApplications);
    }

    public async Task<JobPost?> GetByIdAsync(Guid id)
    {
        return await _dbContext.JobPosts
            .Include(jp => jp.JobType)
            .Include(jp => jp.JobCategory)
            .Include(jp => jp.JobLocation)
            .Include(jp => jp.CreatedBy)
            .Include(jp => jp.JobApplications)
            .SingleOrDefaultAsync(jp => jp.Id == id);
    }

    public async Task<List<JobPost>> GetNewListingsAsync()
    {
        return await _dbContext.JobPosts
            .Where(jp => jp.IsActive)
            .OrderByDescending(jp => jp.CreatedAt)
            .Take(3)
            .Include(jp => jp.JobType)
            .Include(jp => jp.JobCategory)
            .Include(jp => jp.JobLocation)
            .Include(jp => jp.CreatedBy)
            .Include(jp => jp.JobApplications)
            .ToListAsync();
    }

    public async Task RemoveAsync(JobPost entity)
    {
        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid postId, JobPost entity)
    {
        var jobPost = await _dbContext.JobPosts.FindAsync(postId);
        if (jobPost is null)
        {
            return;
        }

        jobPost.Title = entity.Title;
        jobPost.JobTypeId = entity.JobTypeId;
        jobPost.MinSalary = entity.MinSalary;
        jobPost.MaxSalary = entity.MaxSalary;
        jobPost.JobCategoryId = entity.JobCategoryId;
        jobPost.JobLocationId = entity.JobLocationId;
        jobPost.Description = entity.Description;

        jobPost.UpdatedAt = DateTime.UtcNow;

        _dbContext.JobPosts.Update(jobPost);
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid postId)
    {
        var post = await _dbContext.JobPosts.FindAsync(postId);
        if (post != null)
        {
            _dbContext.JobPosts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountAsync()
    {
        return await _dbContext.JobPosts.CountAsync();
    }
}


