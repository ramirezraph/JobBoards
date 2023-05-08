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

    public async Task<IEnumerable<JobCategory>> GetAllAsync()
    {
        var response = await _httpClientService.GetAsync(ApiEndpoint.GetAllJobCategories);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var jobCategories = JsonConvert.DeserializeObject<IEnumerable<JobCategory>>(content);

        return jobCategories;
    }
}