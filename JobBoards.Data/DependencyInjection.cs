using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence;
using JobBoards.Data.Mappings;
using JobBoards.Data.AzureStorage;
using JobBoards.Data.Authentication;
using JobBoards.Data.ApiServices;
using JobBoards.Data.Cors;

namespace JobBoards.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddCorsPolicy(configuration)
            .AddPersistence(configuration)
            .AddIdentity()
            .AddAuthentication(configuration)
            .AddAzureStorage(configuration)
            .AddMappings();
    }

    public static IServiceCollection AddWebAppData(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddPersistence(configuration)
            .AddIdentity()
            .AddWebAuthentication(configuration)
            .AddAzureStorage(configuration)
            .AddMappings()
            .AddRestApiServices();
    }
}
