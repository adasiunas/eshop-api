using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public LoggingMiddleware(ILogger logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogInformation("Request started invoking method: " + next.Method.Name);
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                _logger.LogCritical("Exeption was caught invoking method: " + next.Method.Name);
                _logger.LogCritical("Stack trace:\n" + ex.StackTrace);
            }
            _logger.LogInformation("Method: " + next.Method.Name + " finished");
        }
    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LoggingMiddleware>();
            return builder;
        }
    }
}
