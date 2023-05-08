using System.Text;

namespace JobBoards.Data.ApiServices;

public static class ApiEndpointExtensions
{
    public static string BuildApiUrl(ApiEndpoint endpoint, params object[] uriParameters)
    {
        var template = GetEndpointTemplate(endpoint);
        var url = new StringBuilder(template);

        if (uriParameters == null || uriParameters.Length == 0)
        {
            return url.ToString();
        }

        foreach (var parameter in uriParameters)
        {
            var index = url.ToString().IndexOf("{");
            if (index >= 0)
            {
                url.Replace(url.ToString().Substring(index, url.ToString().IndexOf("}") - index + 1), parameter.ToString());
            }
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
        return attribute.Template;
    }


}