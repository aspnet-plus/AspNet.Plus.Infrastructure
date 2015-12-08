using AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ExceptionMangerSample.RealisticSamples
{
    public class ExceptionFinalizer : IExceptionHandler
    {
        public async Task HandleAsync(IExceptionContext exceptionContext)
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
