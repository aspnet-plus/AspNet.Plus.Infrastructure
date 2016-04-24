// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNet.Plus.Infrastructure.Builder
{
    /// <summary>
    /// Exception Intercept Manager Extensions.
    /// </summary>
    public static class ExceptionInterceptManagerExtensions
    {
        /// <summary>
        /// Adds the exception intercept manager.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddExceptionInterceptManager(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(ExceptionInterceptManager));
            return services;
        }

        /// <summary>
        /// Uses the exception intercept manager.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionInterceptManager(this IApplicationBuilder builder, ExceptionInterceptOptions options = null)
        {
            var middlewareOptions = options ?? new ExceptionInterceptOptions()
            {
                RethrowException = false
            };

            return builder.UseMiddleware<ExceptionInterceptMiddleware>(middlewareOptions);
        }

        /// <summary>
        /// Adds an exception intercept handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="exceptionInterceptHandler">The exception handler.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddExceptionInterceptHandler(this IApplicationBuilder builder, IExceptionInterceptHandler exceptionInterceptHandler)
        {
            var exceptionInterceptManager = builder.ApplicationServices.GetService<ExceptionInterceptManager>();
            if (exceptionInterceptManager != null)
            {
                exceptionInterceptManager.AddExceptionInterceptHandler(exceptionInterceptHandler);
            }

            return builder;
        }

        /// <summary>
        /// Adds an exception intercept handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="exceptionInterceptHandlerType">Type of the exception handler.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddExceptionInterceptHandler(this IApplicationBuilder builder, Type exceptionInterceptHandlerType)
        {
            var exceptionInterceptHandler = builder.ApplicationServices.GetService(exceptionInterceptHandlerType);
            if (exceptionInterceptHandler != null)
            {
                var handler = exceptionInterceptHandler as IExceptionInterceptHandler;
                if (handler != null)
                {
                    builder.AddExceptionInterceptHandler(handler);
                }
            }

            return builder;
        }

        /// <summary>
        /// Adds an exception intercept handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddExceptionInterceptHandler<T>(this IApplicationBuilder builder) where T : IExceptionInterceptHandler
        {
            var exceptionInterceptHandler = builder.ApplicationServices.GetService<T>();
            if (exceptionInterceptHandler != null)
            {
                builder.AddExceptionInterceptHandler(exceptionInterceptHandler);
            }

            return builder;
        }
    }
}
