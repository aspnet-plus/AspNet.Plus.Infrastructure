using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using System.Threading.Tasks;

namespace ExceptionMangerSample.RealisticSamples.ExceptionLoggers
{
    public class ExceptionDbLogger : IExceptionInterceptHandler
    {
        public Task HandleAsync(IExceptionInterceptContext exceptionContext)
        {
            var category = (ExceptionCategory)exceptionContext.Context.Items["exception.category"];
            if (category.Category == ExceptionCategoryType.Unhandled)
            {
                dynamic response = exceptionContext.Context.Items["exception.response"];

                // log whatever to the Database
                // Note: Application Insights may be a more attractive analytical logger than rolling your own.
            }

            return Task.FromResult(0);
        }
    }
}
