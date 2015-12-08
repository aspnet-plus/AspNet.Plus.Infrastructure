// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Threading.Tasks;

namespace AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces
{
    /// <summary>
    /// IExceptionInterceptHandler.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Handles exception asynchronously.
        /// </summary>
        /// <param name="exceptionContext">The exception context.</param>
        /// <returns></returns>
        Task HandleAsync(IExceptionContext exceptionContext);
    }
}
