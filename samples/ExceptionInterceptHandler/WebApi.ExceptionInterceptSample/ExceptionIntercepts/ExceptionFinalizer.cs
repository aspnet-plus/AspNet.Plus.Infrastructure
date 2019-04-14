// ASP.NET.Plus
// Copyright (c) 2019 AspNet.Plus.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 


using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebApi.ExceptionInterceptSample.ExceptionIntercepts
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces.IExceptionInterceptHandler" />
    public class ExceptionFinalizer : IExceptionInterceptHandler
    {
        /// <summary>
        /// Handles exception asynchronously.
        /// </summary>
        /// <param name="exceptionContext">The exception context.</param>
        /// <returns></returns>
        public async Task HandleAsync(IExceptionInterceptContext exceptionContext)
        {
            var category = (ExceptionCategory)exceptionContext.Context.Items["exception.category"];
            dynamic response = exceptionContext.Context.Items["exception.response"];
            dynamic finalResponse = category.DeveloperMode ? response : response.System;

            exceptionContext.Context.Response.StatusCode = (int)category.HttpStatus;
            exceptionContext.Context.Response.ContentType = "application/json";
            await exceptionContext.Context.Response.WriteAsync((string)JsonConvert.SerializeObject(finalResponse, Formatting.Indented));
        }
    }
}
