using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using System.Threading.Tasks;

namespace WebApi.ExceptionInterceptSample.ExceptionIntercepts.ExceptionLoggers
{
    public class ExceptionJIRALogger : IExceptionInterceptHandler
    {
        public Task HandleAsync(IExceptionInterceptContext exceptionContext)
        {
            var category = (ExceptionCategory)exceptionContext.Context.Items["exception.category"];
            if (category.Category == ExceptionCategoryType.Unhandled)
            {
                dynamic response = exceptionContext.Context.Items["exception.response"];

                // log whatever to the JIRA for production issue tracking
            }

            return Task.FromResult(0);
        }
    }
}
