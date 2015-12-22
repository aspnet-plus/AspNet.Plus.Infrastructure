using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebApi.ExceptionInterceptSample.ExceptionIntercepts
{
    public class ExceptionFinalizer : IExceptionInterceptHandler
    {
        public async Task HandleAsync(IExceptionInterceptContext exceptionContext)
        {
            var category = (ExceptionCategory)exceptionContext.Context.Items["exception.category"];
            dynamic response = exceptionContext.Context.Items["exception.response"];
            dynamic finalResponse = category.DeveloperMode ? response : response.System;

            exceptionContext.Context.Response.StatusCode = (int)category.HttpStatus;
            exceptionContext.Context.Response.ContentType = "application/json";
            await exceptionContext.Context.Response.WriteAsync((string)JsonConvert.SerializeObject(finalResponse, Formatting.Indented));
        }
    }
}
