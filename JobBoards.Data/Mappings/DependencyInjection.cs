using Microsoft.Extensions.DependencyInjection;

namespace JobBoards.Data.Mappings;

public static class DependencyInjection
{
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        return services.AddAutoMapper(typeof(MappingProfiles));
    }
}