using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Formatting;
using Newtonsoft.Json.Serialization;

namespace eshopAPI.Utils
{
    public class ExceptionsHandlingFilter : IExceptionFilter
    {
        public async void OnException(ExceptionContext context)
        {
            await HandleException(context);
            context.ExceptionHandled = true;
        }

        private Task HandleException(ExceptionContext context)
        {
            var exception = context.Exception;
            ErrorResponse error = null;
            if (exception is DbUpdateException)
                error = new ErrorResponse(ErrorReasons.DbUpdateException, "Something bad happend.");
            else
                error = new ErrorResponse(exception.Source, exception.Message);

            var result = JsonConvert.SerializeObject(error, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";
            return context.HttpContext.Response.WriteAsync(result);
        }
    }
}
