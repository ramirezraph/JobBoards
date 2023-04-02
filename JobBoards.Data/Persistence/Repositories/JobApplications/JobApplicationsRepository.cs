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
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobType)
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobCategory)
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobLocation)
                .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.Resume)
                .ToListAsync();
    }

    public async Task<List<JobApplication>> GetAllByPostIdAsync(Guid postId)
    {
        return await _dbContext.JobApplications
                .Where(ja => ja.JobPostId == postId)
                .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.Resume)
                .ToListAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _dbContext.JobApplications
                .Include(ja => ja.JobSeeker)
                .SingleOrDefaultAsync(ja => ja.Id == id);
    }

    public async Task<JobApplication?> GetJobSeekerApplicationToJobPostAsync(Guid jobSeekerId, Guid postId)
    {
        return await _dbContext.JobApplications
                 .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.Resume)
                 .Include(ja => ja.JobPost)
                 .SingleOrDefaultAsync(ja => ja.JobPostId == postId && ja.JobSeekerId == jobSeekerId);
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
