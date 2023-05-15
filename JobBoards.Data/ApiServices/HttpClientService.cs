using System.Net.Http.Headers;
using System.Text;
using Azure.Core.Pipeline;
using JobBoards.Data.Authentication;
using JobBoards.Data.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobBoards.Data.ApiServices;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger, IJwtTokenGenerator jwtTokenGenerator)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task Authorize(ApplicationUser user)
    {
        var token = await _jwtTokenGenerator.GenerateToken(user);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<HttpResponseMessage> GetAsync(ApiEndpoint endpoint, params object[] uriParameters)
    {
        var url = ApiEndpointExtensions.BuildApiUrl(endpoint, uriParameters);
        var response = await _httpClient.GetAsync(url);
        return response;
    }

    public async Task<HttpResponseMessage> PostAsync<TRequest>(ApiEndpoint endpoint, TRequest request, params object[] uriParameters)
    {
        var url = ApiEndpointExtensions.BuildApiUrl(endpoint, uriParameters);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        return response;
    }

    public async Task<HttpResponseMessage> PutAsync<TRequest>(ApiEndpoint endpoint, TRequest request, params object[] uriParameters)
    {
        var url = ApiEndpointExtensions.BuildApiUrl(endpoint, uriParameters);
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        // NOTE: Using POST for the time being. Should update to PUT.
        var response = await _httpClient.PostAsync(url, content);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(ApiEndpoint endpoint, params object[] uriParameters)
    {
        var url = ApiEndpointExtensions.BuildApiUrl(endpoint, uriParameters);
        var response = await _httpClient.DeleteAsync(url);
        return response;
    }
}