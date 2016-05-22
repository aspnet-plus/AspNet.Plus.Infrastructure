// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Microsoft.AspNetCore.Http;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;

namespace AspNet.Plus.Infrastructure.ExceptionInterceptHandler
{
    /// <summary>
    /// The exception context that holds the HttpContext and the actual
    /// unhandled exception that occurred in
    /// </summary>
    /// <seealso cref="AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces.IExceptionInterceptContext" />    
    public class ExceptionInterceptContext : IExceptionInterceptContext
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
