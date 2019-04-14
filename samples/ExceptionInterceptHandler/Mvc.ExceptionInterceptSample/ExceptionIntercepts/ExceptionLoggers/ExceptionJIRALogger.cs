// ASP.NET.Plus
// Copyright (c) 2019 AspNet.Plus.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 


using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using System.Threading.Tasks;

namespace Mvc.ExceptionInterceptSample.ExceptionIntercepts.ExceptionLoggers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces.IExceptionInterceptHandler" />
    public class ExceptionJIRALogger : IExceptionInterceptHandler
    {
        /// <summary>
        /// Handles exception asynchronously.
        /// </summary>
        /// <param name="exceptionContext">The exception context.</param>
        /// <returns></returns>
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
