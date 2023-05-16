namespace JobBoards.Data.ApiServices;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(ApiEndpoint endpoint, params object[] uriParameters);
    Task<HttpResponseMessage> PostAsync<TRequest>(ApiEndpoint endpoint, TRequest request, params object[] uriParameters);
    Task<HttpResponseMessage> PutAsync<TRequest>(ApiEndpoint endpoint, TRequest request, params object[] uriParameters);
    Task<HttpResponseMessage> DeleteAsync(ApiEndpoint endpoint, params object[] uriParameters);
}