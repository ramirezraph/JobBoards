using JobBoards.Data.Contracts.JobCategory;
using JobBoards.Data.Entities;
using Microsoft.AspNetCore.Http;
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
        var response = await _httpClientService
            .GetAsync(ApiEndpoint.GetAllJobCategories);

        if (!response.IsSuccessStatusCode)
        {
            return new();
        }

        var content = await response.Content.ReadAsStringAsync();
        var jobCategories = JsonConvert.DeserializeObject<List<JobCategory>>(content);

        return jobCategories;
    }

    public async Task<JobCategory?> GetByIdAsync(Guid id)
    {
        var response = await _httpClientService.GetAsync(ApiEndpoint.GetJobCategoryById, new { Id = id });
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var jobCategory = JsonConvert.DeserializeObject<JobCategory>(content);

        return jobCategory;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateJobCategoryRequest updatedJobCategory)
    {
        var response = await _httpClientService
            .PostAsync(ApiEndpoint.UpdateJobCategory, updatedJobCategory, new { Id = id });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClientService
            .DeleteAsync(ApiEndpoint.DeleteJobCategory, new { Id = id });
        return response.IsSuccessStatusCode;
    }

    public async Task<JobCategory?> CreateAsync(CreateJobCategoryRequest newJobCategory)
    {
        var response = await _httpClientService
            .PostAsync(ApiEndpoint.CreateJobCategory, newJobCategory);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var jobCategory = JsonConvert.DeserializeObject<JobCategory>(content);

        return jobCategory;
    }
}