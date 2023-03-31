using JobBoards.Data.Persistence.Initialization;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDbInitializer(this IServiceCollection services)
    {
        return services.AddScoped<IDbInitializer, DbInitializer>();
    }
}