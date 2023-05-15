using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Entities;

namespace JobBoards.Data.ApiServices.JobCategoryAPI;

public interface IJobCategoryAPI
{
    Task<List<JobCategory>> GetAllAsync();
    Task<JobCategory?> GetByIdAsync(Guid id);
    Task<JobCategory?> CreateAsync(CreateJobCategoryRequest newJobCategory);
    Task<JobCategory?> UpdateAsync(Guid id, UpdateJobCategoryRequest updatedJobCategory);
    Task DeleteAsync(Guid id);
}