using System.Reflection;
using System.Text;

namespace JobBoards.Data.ApiServices;

public static class ApiUtils
{
    public static string BuildApiUrl(ApiEndpoint endpoint, object? uriParameters)
    {
        var template = GetEndpointTemplate(endpoint);
        var url = new StringBuilder(template);

        if (uriParameters == null)
        {
            return url.ToString();
        }

        Type objectType = uriParameters.GetType();
        PropertyInfo[] properties = objectType.GetProperties();

        foreach (var property in properties)
        {
            string propertyName = property.Name.ToLowerInvariant();
            object? propertyValue = property.GetValue(uriParameters);

            url.Replace("{" + propertyName + "}", propertyValue?.ToString());
        }

        return url.ToString();
    }

    private static string GetEndpointTemplate(ApiEndpoint endpoint)
    {
        var field = endpoint.GetType().GetField(endpoint.ToString());
        var attribute = field?.GetCustomAttributes(typeof(EndpointAttribute), false).FirstOrDefault() as EndpointAttribute;
        if (attribute == null)
        {
            throw new Exception($"Missing endpoint attribute for endpoint '{endpoint}'.");
        }
        return attribute.Template.ToLowerInvariant();
    }


}