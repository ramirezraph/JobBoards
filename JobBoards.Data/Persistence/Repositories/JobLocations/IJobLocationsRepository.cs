using JobBoards.Data.Entities;
using JobBoards.Data.Persistence.Repositories.Common;

namespace JobBoards.Data.Persistence.Repositories.JobLocations;

public interface IJobLocationsRepository : IRepository<JobLocation>
{
    Task UpdateAsync(Guid id, JobLocation entity);
}