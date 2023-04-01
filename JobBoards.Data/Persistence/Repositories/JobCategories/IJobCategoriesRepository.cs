using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Common;

namespace JobBoards.Data.Persistence.Repositories.JobCategories;

public interface IJobCategoriesRepository : IRepository<JobCategory>
{
    Task UpdateAsync(Guid id, JobCategory entity);
}