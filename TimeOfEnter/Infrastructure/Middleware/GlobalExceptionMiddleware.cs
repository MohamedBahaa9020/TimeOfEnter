using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace TimeOfEnter.Infrastructure.Middleware;
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        logger.LogError(traceId,
            exception,
            "Unhandled exception occurred. Path: {Path}",
            context.Request.Path
            );

        var statusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Internal Server Error",
            Detail = exception.Message,
            Instance = context.Request.Path,

        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonConvert.SerializeObject(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
