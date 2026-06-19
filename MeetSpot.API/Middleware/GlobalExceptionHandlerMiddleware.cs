using System.Net;
using System.Text.Json;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Helpers;

namespace MeetSpot.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var response = ex switch
        {
            NotFoundException e => (StatusCode: 404, Response: ResponseModelHelper<string>.NotFoundResult(e.Message)),
            ConflictException e => (StatusCode: 409, Response: ResponseModelHelper<string>.ConflictResult(e.Message)),
            UnauthorizedException e => (StatusCode: 401, Response: ResponseModelHelper<string>.UnauthorizedResult(e.Message)),
            BadRequestException e => (StatusCode: 400, Response: ResponseModelHelper<string>.BadRequestResult(e.Message)),
            _ => (StatusCode: 500, Response: ResponseModelHelper<string>.ErrorResult("An unexpected error occurred"))
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response.Response));
    }
}