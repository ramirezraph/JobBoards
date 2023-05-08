using JobBoards.Data.Entities;

namespace JobBoards.Data.ApiServices.JobCategoryAPI;

public interface IJobCategoryAPI
{
    Task<IEnumerable<JobCategory>> GetAllAsync();
}