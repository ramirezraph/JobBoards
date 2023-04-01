using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Common;

namespace JobBoards.Data.Persistence.Repositories.JobPosts;

public interface IJobPostsRepository : IRepository<JobPost>
{
    Task UpdateAsync(Guid postId, JobPost entity);
}