using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TimeOfEnter.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next , ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context )
        {
            try {

                await next(context);
            }
            catch(Exception ex ) {
                await HandleExceptionAsync(context ,ex);


            }
        }

        private  Task HandleExceptionAsync(HttpContext context, Exception exception)
        {


            //var errorRespnse = new
            //{
            //    message = "An error occurred while processing your request.",
            //    details = exception.Message
            //};
            var traceId = context.TraceIdentifier;

            logger.LogError(traceId,
                exception,
                "Unhandled exception occurred. Path: {Path}",
                context.Request.Path
                );

            var statusCode = (int) HttpStatusCode.InternalServerError;
            

            var problemDetails = new ProblemDetails
            {
                //Type= "https://httpstatuses.com/500",
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
}
