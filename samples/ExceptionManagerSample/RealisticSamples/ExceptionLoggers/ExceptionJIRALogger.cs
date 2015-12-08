using AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces;
using System.Threading.Tasks;

namespace ExceptionMangerSample.RealisticSamples.ExceptionLoggers
{
    public class ExceptionJIRALogger : IExceptionHandler
    {
        public Task HandleAsync(IExceptionContext exceptionContext)
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
