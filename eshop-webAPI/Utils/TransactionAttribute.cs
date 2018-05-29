using eshopAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Utils
{
    public class TransactionAttribute : TypeFilterAttribute
    {
        public TransactionAttribute() : base(typeof(TransactionAttributeImpl))
        {
        }

        public class TransactionAttributeImpl : IAsyncResultFilter
        {
            private readonly ShopContext _shopContext;

            public TransactionAttributeImpl(ShopContext shopContext)
            {
                _shopContext = shopContext;
            }

            public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
            {
                var executionContext = await next();
                if (executionContext.HttpContext.Response.StatusCode >= 200 &&
                    executionContext.HttpContext.Response.StatusCode < 300)
                {
                    try
                    {
                        await _shopContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        executionContext.HttpContext.Response.StatusCode = 500;
                        await executionContext.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                            new ErrorResponse(ErrorReasons.DbUpdateException, e.Message)));
                    }
                }
            }
        }
    }
}
