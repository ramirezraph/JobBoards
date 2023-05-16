using System.Net.Http.Headers;
using System.Text;
using JobBoards.Data.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobBoards.Data.ApiServices;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
        var response = await _httpClient.PutAsync(url, content);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(ApiEndpoint endpoint, params object[] uriParameters)
    {
        var url = ApiEndpointExtensions.BuildApiUrl(endpoint, uriParameters);
        var response = await _httpClient.DeleteAsync(url);
        return response;
    }
}