using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Repositories.JobApplications;

public class JobApplicationsRepository : IJobApplicationsRepository
{
    private readonly JobBoardsDbContext _dbContext;

    public JobApplicationsRepository(JobBoardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(JobApplication entity)
    {
        await _dbContext.JobApplications.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<JobApplication>> GetAllAsync()
    {
        return await _dbContext.JobApplications.ToListAsync();
    }

    public async Task<List<JobApplication>> GetAllByJobSeekerIdAsync(Guid jobSeekerId)
    {
        return await _dbContext.JobApplications
                .Where(ja => ja.JobSeekerId == jobSeekerId)
                .ToListAsync();
    }

    public async Task<List<JobApplication>> GetAllByPostIdAsync(Guid postId)
    {
        return await _dbContext.JobApplications
                .Where(ja => ja.JobPostId == postId)
                .ToListAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _dbContext.JobApplications
                .Include(ja => ja.JobSeeker)
                .SingleOrDefaultAsync(ja => ja.Id == id);
    }

    public Task RemoveAsync(JobApplication entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStatusAsync(string newStatus)
    {
        throw new NotImplementedException();
    }
}
