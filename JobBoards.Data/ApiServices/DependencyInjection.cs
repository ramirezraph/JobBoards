using JobBoards.Data.ApiServices.JobCategoryAPI;
using JobBoards.Data.Authentication;
using JobBoards.Data.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;

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
                    var session = httpContextAccessor.HttpContext?.Session;

                    if (session != null)
                    {
                        var jwt = session.GetString("JWT");
                        if (!string.IsNullOrEmpty(jwt))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                        }
                    }
                });

        services.AddTransient<IJobCategoryAPI, JobCategoryAPI.JobCategoryAPI>();

        return services;
    }
}