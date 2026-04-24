namespace EndpointGuardian.Api.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next(context);
        var duration = DateTime.UtcNow - start;

        Console.WriteLine(
            $"[{context.Request.method}] {context.Request.Path} -> {context.Response.StatusCode} in {duration.TotalMilliseconds} ms");
    }
}