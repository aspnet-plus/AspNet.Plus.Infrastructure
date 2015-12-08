using AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace ExceptionMangerSample.RealisticSamples
{
    public class ExceptionInitializer : IExceptionHandler
    {
        private readonly ExceptionCategorizer _exceptionCategorizer;

        public ExceptionInitializer(ExceptionCategorizer exceptionCategorizer)
        {
            _exceptionCategorizer = exceptionCategorizer;
        }

        public Task HandleAsync(IExceptionContext exceptionContext)
        {
            var category = _exceptionCategorizer.Categorizer(exceptionContext.Exception);
            dynamic response = new ExpandoObject();

            response.System = new Dictionary<string, string>();
            response.System["Tracking Id"] = Guid.NewGuid().ToString();
            response.System["Timestamp"] = DateTimeOffset.Now.ToString();
            response.System["Message"] = category.ErrorMessage;
            response.System["Execution"] = "Global";

            if (exceptionContext.Context.Request != null)
            {
                response.System["Execution"] = "Request";

                if (category.Category == ExceptionCategoryType.Unhandled)
                {
                    response.Developer = new Dictionary<string, string>();
                    response.Developer["RequestMethod"] = exceptionContext.Context.Request.Method;
                    response.Developer["Uri"] = $"{exceptionContext.Context.Request.Scheme}:{exceptionContext.Context.Request.Host}{exceptionContext.Context.Request.Path}";
                    response.Developer["ExceptionType"] = exceptionContext.Exception.GetType().FullName;
                    response.Developer["StackTrace"] = exceptionContext.Exception.StackTrace.Trim();
                }
            }

            exceptionContext.Context.Items["exception.category"] = category;
            exceptionContext.Context.Items["exception.response"] = response;

            return Task.FromResult(0);
        }
    }
}
