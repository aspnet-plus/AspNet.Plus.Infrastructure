// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;

namespace AspNet.Plus.Infrastructure.ExceptionInterceptHandler
{
    /// <summary>
    /// The Exception Intercept Manager traps all unhandled exception and 
    /// delegates in context, the exception each ExceptiontInterceptHandler in sequence, if any.
    /// </summary>
    public class ExceptionInterceptManager
    {
        private readonly IList<IExceptionInterceptHandler> _exceptionInterceptHandlers;

        /// <summary>
        /// </summary>
        public ExceptionInterceptManager()
        {
            _exceptionInterceptHandlers = new List<IExceptionInterceptHandler>();
        }

        /// <summary>
        /// Intercepts the specified context.
        /// </summary>
        /// <param name="exceptionContext">The context.</param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="System.AggregateException"></exception>
        public async Task InterceptAsync(IExceptionInterceptContext exceptionContext)
        {
            var handlerExecutionExceptions = new List<Exception>();
            try
            {
                foreach (var handler in _exceptionInterceptHandlers)
                {
                    try
                    {
                        await handler.HandleAsync(exceptionContext);
                    }
                    catch (Exception ex)
                    {
                        handlerExecutionExceptions.Add(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                handlerExecutionExceptions.Add(ex);
            }
            finally
            {
                if (handlerExecutionExceptions.Any())
                {
                    throw new AggregateException(handlerExecutionExceptions);
                }
            }
        }

        /// <summary>
        /// Adds and exception intercept handler.
        /// </summary>
        /// <param name="exceptionInterceptHandler">The exception intercept handler.</param>
        public void AddExceptionInterceptHandler(IExceptionInterceptHandler exceptionInterceptHandler)
        {
            _exceptionInterceptHandlers.Add(exceptionInterceptHandler);
        }
    }
}
