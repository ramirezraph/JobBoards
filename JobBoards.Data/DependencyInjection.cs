using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JobBoards.Data.Identity;
using JobBoards.Data.Persistence;

namespace JobBoards.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddIdentity();

        return services;
    }
}
