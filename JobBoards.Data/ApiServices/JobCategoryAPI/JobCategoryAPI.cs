using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Entities;
using Newtonsoft.Json;

namespace JobBoards.Data.ApiServices.JobCategoryAPI;

public class JobCategoryAPI : IJobCategoryAPI
{
    private readonly IHttpClientService _httpClientService;

    public JobCategoryAPI(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<List<JobCategory>> GetAllAsync()
    {
        var response = await _httpClientService.GetAsync(ApiEndpoint.GetAllJobCategories);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var jobCategories = JsonConvert.DeserializeObject<List<JobCategory>>(content);

        return jobCategories;
    }

    public async Task<JobCategory?> GetByIdAsync(Guid id)
    {
        var response = await _httpClientService.GetAsync(ApiEndpoint.GetJobCategoryById, id);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var jobCategory = JsonConvert.DeserializeObject<JobCategory>(content);

        return jobCategory;
    }

    public async Task<JobCategory?> UpdateAsync(Guid id, UpdateJobCategoryRequest updatedJobCategory)
    {
        var response = await _httpClientService.PutAsync(ApiEndpoint.UpdateJobCategory, updatedJobCategory, id);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var jobCategory = JsonConvert.DeserializeObject<JobCategory>(content);

        return jobCategory;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<JobCategory?> CreateAsync(CreateJobCategoryRequest newJobCategory)
    {
        throw new NotImplementedException();
    }
}