using AutoMapper;
using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace JobBoards.Data.Persistence.Repositories.Resumes
{
    public class ResumesRepository : IResumesRepository
    {
        private readonly JobBoardsDbContext _dbContext;
        public ResumesRepository(JobBoardsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Resume entity)
        {
            await _dbContext.Resumes.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Resume>> GetAllAsync()
        {
            return await _dbContext.Resumes.ToListAsync();
        }

        public async Task<Resume?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Resumes.FindAsync(id);
        }

        public async Task RemoveAsync(Resume entity)
        {
            _dbContext.Resumes.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(Guid id, Resume entity)
        {
            throw new NotImplementedException();
        }
    }
}
