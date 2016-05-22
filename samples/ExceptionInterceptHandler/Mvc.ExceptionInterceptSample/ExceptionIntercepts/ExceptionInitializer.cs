// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Mvc.ExceptionInterceptSample.ExceptionIntercepts
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces.IExceptionInterceptHandler" />
    public class ExceptionInitializer : IExceptionInterceptHandler
    {
        private readonly ExceptionCategorizer _exceptionCategorizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInitializer"/> class.
        /// </summary>
        /// <param name="exceptionCategorizer">The exception categorizer.</param>
        public ExceptionInitializer(ExceptionCategorizer exceptionCategorizer)
        {
            _exceptionCategorizer = exceptionCategorizer;
        }

        /// <summary>
        /// Handles exception asynchronously.
        /// </summary>
        /// <param name="exceptionContext">The exception context.</param>
        /// <returns></returns>
        public Task HandleAsync(IExceptionInterceptContext exceptionContext)
        {
            var category = _exceptionCategorizer.Categorizer(exceptionContext.Exception);
            dynamic response = new ExpandoObject();

            response.Status = category.HttpStatus;
            response.TrackingId = Guid.NewGuid().ToString();
            response.Timestamp = DateTimeOffset.Now.ToString();
            response.Message = category.ErrorMessage;
            response.Execution = "Global";

            if (exceptionContext.Context.Request != null)
            {
                response.Execution = "Request";

                if (category.Category == ExceptionCategoryType.Unhandled)
                {
                    response.Developer = new ExpandoObject();
                    response.Developer.RequestMethod = exceptionContext.Context.Request.Method;
                    response.Developer.Uri = $"{exceptionContext.Context.Request.Scheme}:{exceptionContext.Context.Request.Host}{exceptionContext.Context.Request.Path}";
                    response.Developer.ExceptionType = exceptionContext.Exception.GetType().FullName;
                    response.Developer.StackTrace = exceptionContext.Exception.StackTrace.Trim();
                }
            }

            exceptionContext.Context.Items["exception.category"] = category;
            exceptionContext.Context.Items["exception.response"] = response;

            return Task.FromResult(0);
        }
    }
}
