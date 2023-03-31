using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Repositories.JobLocations;

public class JobLocationsRepository : IJobLocationsRepository
{
    private readonly JobBoardsDbContext _dbContext;

    public JobLocationsRepository(JobBoardsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(JobLocation entity)
    {
        await _dbContext.JobLocations.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<JobLocation>> GetAllAsync()
    {
        return await _dbContext.JobLocations.ToListAsync();
    }

    public async Task<JobLocation?> GetByIdAsync(Guid id)
    {
        return await _dbContext.JobLocations.FindAsync(id);
    }

    public async Task RemoveAsync(JobLocation entity)
    {
        _dbContext.JobLocations.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, JobLocation entity)
    {
        var jobLocation = await _dbContext.JobLocations.FindAsync(id);
        if (jobLocation is null)
        {
            throw new Exception("Trying to update job location that doesn't exists.");
        }

        jobLocation.City = entity.City;
        jobLocation.Country = entity.Country;
        jobLocation.UpdatedAt = DateTime.UtcNow;

        _dbContext.JobLocations.Update(jobLocation);

        await _dbContext.SaveChangesAsync();
    }
}