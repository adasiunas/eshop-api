using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils
{
    public class AntiforgeryMiddleware : IMiddleware
    {
        private IAntiforgery antiforgery;

        public AntiforgeryMiddleware(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = antiforgery.GetAndStoreTokens(context);
            context.Response.Headers["X-CSRF-COOKIE"] = token.RequestToken;
            return next.Invoke(context);
        }
    }

    public static class AntiforgeryMiddlewareExtensions
    {
        public static IApplicationBuilder UseAntiforgeryMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AntiforgeryMiddleware>();
            return builder;
        }
    }
}
