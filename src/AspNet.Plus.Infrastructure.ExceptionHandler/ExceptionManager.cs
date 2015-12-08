// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;

namespace AspNet.Plus.Infrastructure.ExceptionHandler
{
    /// <summary>
    /// The HTTP Exception Intercept Manager that delegates an unhandled exception to
    /// defined ExceptionInterceptHandlers in sequence, if any.
    /// </summary>
    public class ExceptionManager
    {
        private readonly IList<IExceptionHandler> _exceptionHandlers;

        /// <summary>
        /// </summary>
        public ExceptionManager()
        {
            _exceptionHandlers = new List<IExceptionHandler>();
        }

        /// <summary>
        /// Intercepts the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="System.AggregateException"></exception>
        public async Task InterceptExceptionAsync(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var exceptionContext = new ExceptionContext() { Context = context, Exception = feature?.Error };
            var handlerExecutionExceptions = new List<Exception>();

            try
            {
                foreach (var handler in _exceptionHandlers)
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
        /// Adds the exception intercept.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        public void AddExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
        }
    }
}
