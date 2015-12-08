// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Microsoft.AspNet.Http;

namespace AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces
{
    public interface IExceptionContext
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        HttpContext Context { get; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        Exception Exception { get; }
    }
}
