using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobBoards.Data.ApiServices;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> GetAsync(ApiEndpoint endpoint, params object[] uriParameters)
    {
        var url = ApiEndpointExtensions.BuildApiUrl(endpoint, uriParameters);
        var response = await _httpClient.GetAsync(url);
        return response;
    }
}