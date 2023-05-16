using JobBoards.Data.ApiServices.JobCategoryAPI;
using Microsoft.AspNetCore.Http;
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
                    client.BaseAddress = new Uri("https://localhost:7227/api/");

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