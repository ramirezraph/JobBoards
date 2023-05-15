using JobBoards.Data.ApiServices.JobCategoryAPI;
using Microsoft.Extensions.DependencyInjection;

namespace JobBoards.Data.ApiServices;

public static class DependencyInjection
{
    public static IServiceCollection AddRestApiServices(this IServiceCollection services)
    {
        services.AddHttpClient<IHttpClientService, HttpClientService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7227/api/");
        });

        services.AddTransient<IJobCategoryAPI, JobCategoryAPI.JobCategoryAPI>();

        return services;
    }
}