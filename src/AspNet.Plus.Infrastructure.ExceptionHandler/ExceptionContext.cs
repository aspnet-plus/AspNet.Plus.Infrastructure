// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Microsoft.AspNet.Http;
using AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces;

namespace AspNet.Plus.Infrastructure.ExceptionHandler
{
    /// <summary>
    /// The exception context that holds the HttpContext and the actual
    /// unhandled exception that occurred in
    /// </summary>
    /// <seealso cref="AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces.IExceptionContext" />
    public class ExceptionContext : IExceptionContext
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public HttpContext Context { get; set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }
    }
}
