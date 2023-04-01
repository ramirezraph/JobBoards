using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using JobBoards.Data.Persistence.Repositories.Resumes;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Repositories.JobSeekers;

public class JobSeekersRepository : IJobSeekersRepository
{
    private readonly JobBoardsDbContext _dbContext;
    private readonly IResumesRepository _resumesRepository;

    public JobSeekersRepository(JobBoardsDbContext dbContext, IResumesRepository resumesRepository)
    {
        _dbContext = dbContext;
        _resumesRepository = resumesRepository;
    }

    public async Task AddAsync(JobSeeker entity)
    {
        await _dbContext.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<JobSeeker>> GetAllAsync()
    {
        return await _dbContext.JobSeekers.ToListAsync();
    }

    public async Task<JobSeeker?> GetByIdAsync(Guid id)
    {
        return await _dbContext.JobSeekers.FindAsync(id);
    }

    public async Task RemoveAsync(JobSeeker entity)
    {
        _dbContext.JobSeekers.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateResume(Guid id, string resumeDownloadLink)
    {
        var jobseeker = await _dbContext.JobSeekers.FindAsync(id);
        if (jobseeker is null)
        {
            throw new Exception("Trying to update job location that doesn't exists.");
        }

        // Remove the old resume
        var oldResume = await _resumesRepository.GetByIdAsync(jobseeker.ResumeId);
        if (oldResume is not null)
        {
            await _resumesRepository.RemoveAsync(oldResume);
        }

        // Add the new resume
        var newResume = Resume.CreateNew(jobseeker.Id, resumeDownloadLink);
        await _resumesRepository.AddAsync(newResume);
        jobseeker.ResumeId = newResume.Id;

        _dbContext.JobSeekers.Update(jobseeker);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<JobSeeker?> RegisterUserAsJobSeeker(string userId)
    {
        var newJobSeeker = JobSeeker.RegisterUserAsJobseeker(userId);
        await AddAsync(newJobSeeker);

        return newJobSeeker;
    }

    public Task<JobSeeker?> GetJobSeekerProfileByUserId(string userId)
    {
        return _dbContext.JobSeekers.SingleOrDefaultAsync(js => js.UserId == userId);
    }
}