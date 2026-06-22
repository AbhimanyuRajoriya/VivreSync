using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace VivreSync.Shared.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            var response = new
            {
                message = exception.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}