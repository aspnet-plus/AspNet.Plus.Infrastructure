// ASP.NET.Plus
// Copyright (c) 2016 AspNet.Plus.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Threading.Tasks;

namespace AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces
{
    /// <summary>
    /// IExceptionInterceptHandler.
    /// </summary>
    public interface IExceptionInterceptHandler
    {
        /// <summary>
        /// Handles exception asynchronously.
        /// </summary>
        /// <param name="exceptionContext">The exception context.</param>
        /// <returns></returns>
        Task HandleAsync(IExceptionInterceptContext exceptionContext);
    }
}
