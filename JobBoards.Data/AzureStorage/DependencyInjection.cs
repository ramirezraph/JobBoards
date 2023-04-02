using Azure.Storage.Blobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobBoards.Data.AzureStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(x =>
        {
            string? connectionString = configuration.GetConnectionString("AzureStorageConnection");

            if (connectionString is null)
            {
                throw new ArgumentNullException("Azure Connection string is not configured.");
            }

            return new BlobServiceClient(connectionString);
        });

        return services;
    }
}