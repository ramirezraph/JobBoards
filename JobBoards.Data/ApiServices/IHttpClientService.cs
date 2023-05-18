namespace JobBoards.Data.ApiServices;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(ApiEndpoint endpoint, object? uriParameters = null);
    Task<HttpResponseMessage> PostAsync<TRequest>(ApiEndpoint endpoint, TRequest request, object? uriParameters = null);
    Task<HttpResponseMessage> PutAsync<TRequest>(ApiEndpoint endpoint, TRequest request, object? uriParameters = null);
    Task<HttpResponseMessage> DeleteAsync(ApiEndpoint endpoint, object? uriParameters = null);
}