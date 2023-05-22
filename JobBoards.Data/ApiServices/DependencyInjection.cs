using JobBoards.Data.ApiServices.JobCategoryAPI;
using JobBoards.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace JobBoards.Data.ApiServices;

public static class DependencyInjection
{
    public static IServiceCollection AddRestApiServices(this IServiceCollection services)
    {
        services.AddHttpClient<IHttpClientService, HttpClientService>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.BaseAddress = new Uri("http://localhost:5153/api/");

                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var apiKeyFromConfiguration = configuration.GetValue<string>(ApiConstants.ApiKeyName);
                    if (!string.IsNullOrEmpty(apiKeyFromConfiguration))
                    {
                        client.DefaultRequestHeaders.Add(ApiConstants.ApiKeyHeader, apiKeyFromConfiguration);
                    }

                    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    var jwtCookie = httpContextAccessor?.HttpContext?.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(jwtCookie))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtCookie);
                    }
                });

        services.AddTransient<IJobCategoryAPI, JobCategoryAPI.JobCategoryAPI>();

        return services;
    }
}