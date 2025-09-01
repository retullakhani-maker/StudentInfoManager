using Core.Model;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentInfoManager.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                // Use scoped DbContext safely
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var errorLog = new ErrorLog
                    {
                        Message = ex.Message ?? "No message",
                        StackTrace = ex.StackTrace ?? "No stack trace",
                        Path = context.Request.Path.HasValue ? context.Request.Path.Value : "Unknown",
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.ErrorLogs.Add(errorLog);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(dbEx, "Failed to save error log to database.");
                    }
                }

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Something went wrong.\"}");
            }
        }
    }


    // Extension method
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
