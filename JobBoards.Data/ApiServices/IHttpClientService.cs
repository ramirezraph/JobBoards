namespace JobBoards.Data.ApiServices;

public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(ApiEndpoint endpoint, params object[] uriParameters);
}