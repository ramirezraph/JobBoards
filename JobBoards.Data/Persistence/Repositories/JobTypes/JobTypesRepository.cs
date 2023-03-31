using AutoMapper;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobBoards.Data.Persistence.Repositories.JobTypes
{
    public class JobTypesRepository : IJobTypesRepository
    {
        private readonly JobBoardsDbContext _dbContext;
        public JobTypesRepository(JobBoardsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(JobType entity)
        {
            await _dbContext.JobTypes.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<JobType>> GetAllAsync()
        {
            return await _dbContext.JobTypes.ToListAsync();
        }

        public async Task<JobType?> GetByIdAsync(Guid id)
        {
            return await _dbContext.JobTypes.FindAsync(id);
        }

        public async Task RemoveAsync(JobType entity)
        {
            _dbContext.JobTypes.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }


        public async Task UpdateAsync(Guid id, JobType entity)
        {
            var jobType = await _dbContext.JobTypes.FindAsync(id);
            if (jobType is null)
            {
                throw new Exception("Trying to update job type that doesn't exit.");
            }

            jobType.Name = entity.Name;
            jobType.Description = entity.Description;
            jobType.UpdatedAt = DateTime.UtcNow;

            _dbContext.JobTypes.Update(jobType);

            await _dbContext.SaveChangesAsync();
        }
    }
}
