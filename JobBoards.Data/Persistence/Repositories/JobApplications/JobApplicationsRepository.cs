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
        return await _dbContext.JobApplications
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobType)
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobCategory)
                .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobLocation)
                .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.Resume)
                .OrderByDescending(ja => ja.UpdatedAt)
                    .ThenByDescending(ja => ja.CreatedAt)
                .ToListAsync();
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
                .OrderByDescending(ja => ja.UpdatedAt)
                    .ThenByDescending(ja => ja.CreatedAt)
                .ToListAsync();
    }

    public async Task<List<JobApplication>> GetAllByPostIdAsync(Guid postId)
    {
        return await _dbContext.JobApplications
                .Where(ja => ja.JobPostId == postId)
                .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.Resume)
                .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.User)
                .ToListAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _dbContext.JobApplications
                .Include(ja => ja.JobPost)
                .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.User)
                .SingleOrDefaultAsync(ja => ja.Id == id);
    }

    public async Task<JobApplication?> GetJobSeekerApplicationToJobPostAsync(Guid jobSeekerId, Guid postId)
    {
        return await _dbContext.JobApplications
                 .Include(ja => ja.JobSeeker)
                    .ThenInclude(js => js.Resume)
                 .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobType)
                 .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobLocation)
                 .Include(ja => ja.JobPost)
                    .ThenInclude(jp => jp.JobCategory)
                 .SingleOrDefaultAsync(ja => ja.JobPostId == postId && ja.JobSeekerId == jobSeekerId);
    }

    public async Task RemoveAsync(JobApplication entity)
    {
        _dbContext.JobApplications.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task WithdrawAsync(Guid jobApplicationId)
    {
        var jobApplication = await _dbContext.JobApplications.SingleOrDefaultAsync(ja => ja.Id == jobApplicationId);
        if (jobApplication is not null)
        {
            jobApplication.Status = "Withdrawn";
            jobApplication.UpdatedAt = DateTime.Now;

            _dbContext.JobApplications.Update(jobApplication);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateStatusAsync(Guid id, string newStatus)
    {
        var jobApplication = await _dbContext.JobApplications.SingleOrDefaultAsync(ja => ja.Id == id);
        if (jobApplication is not null)
        {
            jobApplication.Status = newStatus;
            jobApplication.UpdatedAt = DateTime.Now;

            _dbContext.JobApplications.Update(jobApplication);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<JobApplication>> GetThreeRecentJobApplicationAsync()
    {
        return await _dbContext.JobApplications
            .OrderByDescending(jp => jp.CreatedAt)
            .Take(3)
            .Include(ja => ja.JobSeeker)
                .ThenInclude(js => js.User)
            .Include(ja => ja.JobPost)
            .ToListAsync();
    }

    public async Task<int> GetNumberOfJobApplicationsTodayAsync()
    {
        return await _dbContext.JobApplications
            .Where(ja => ja.CreatedAt >= DateTime.Today && ja.CreatedAt < DateTime.Today.AddDays(1))
            .CountAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _dbContext.JobApplications.CountAsync();
    }
}
