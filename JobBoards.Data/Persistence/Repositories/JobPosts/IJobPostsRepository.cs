using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Common;

namespace JobBoards.Data.Persistence.Repositories.JobPosts;

public interface IJobPostsRepository : IRepository<JobPost>
{
    Task UpdateAsync(Guid postId, JobPost entity);
    Task<List<JobPost>> GetNewListingsAsync();
    Task SoftDeleteAsync(Guid postId);
    Task DeleteAsync(Guid postId);
    IQueryable<JobPost> GetAllQueryable();
    Task<int> GetCountAsync();
    Task<List<JobPost>> GetAllDeletedAsync();
    Task<int> GetCountOfDeletedPostsAsync();
}