using System;
using System.Net;
using System.Text.Json;
using API.Errors;


namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(IHostEnvironment env, RequestDelegate next)
    {
        _env = env;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _env);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment _env)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = _env.IsDevelopment()
            ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
            : new ApiErrorResponse(context.Response.StatusCode, ex.Message, "Internal server error");
            

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}
