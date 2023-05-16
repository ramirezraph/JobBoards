using JobBoards.Data.Common;

namespace JobBoards.Api.Middlewares;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    public ApiKeyAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiConstants.ApiKeyHeader, out var apiKeyFromRequestHeader))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }
        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var apiKeyFromConfiguration = configuration.GetValue<string>(ApiConstants.ApiKeyName);
        if (apiKeyFromConfiguration is null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        if (!apiKeyFromConfiguration.Equals(apiKeyFromRequestHeader))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is invalid.");
            return;
        }

        await _next(context);
    }
}
