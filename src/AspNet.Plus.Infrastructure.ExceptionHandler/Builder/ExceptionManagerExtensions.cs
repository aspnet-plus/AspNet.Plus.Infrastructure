// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using AspNet.Plus.Infrastructure.ExceptionHandler;
using AspNet.Plus.Infrastructure.ExceptionHandler.Interfaces;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNet.Plus.Infrastructure.Builder
{
    /// <summary>
    /// Exception Manager Extensions.
    /// </summary>
    public static class ExceptionManagerExtensions
    {
        /// <summary>
        /// Adds the exception manager.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddExceptionManager(this IServiceCollection services)
        {
            services.TryAddSingleton(typeof(ExceptionManager));
            return services;
        }

        /// <summary>
        /// Uses the exception manager.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionManager(this IApplicationBuilder builder)
        {
            var exceptionManager = builder.ApplicationServices.GetService<ExceptionManager>();
            if (exceptionManager != null)
            {
                builder.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(exceptionManager.InterceptExceptionAsync);
                });
            }

            return builder;
        }

        /// <summary>
        /// Adds an exception handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder builder, IExceptionHandler exceptionHandler)
        {
            var exceptionManager = builder.ApplicationServices.GetService<ExceptionManager>();
            if (exceptionManager != null)
            {
                exceptionManager.AddExceptionHandler(exceptionHandler);
            }

            return builder;
        }

        /// <summary>
        /// Adds an exception handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="exceptionHandlerType">Type of the exception handler.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder builder, Type exceptionHandlerType)
        {
            var exceptionHandler = builder.ApplicationServices.GetService(exceptionHandlerType);
            if (exceptionHandler != null)
            {
                var handler = exceptionHandler as IExceptionHandler;
                if (handler != null)
                {
                    builder.AddExceptionHandler(handler);
                }
            }

            return builder;
        }

        /// <summary>
        /// Adds an exception handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IApplicationBuilder AddExceptionHandler<T>(this IApplicationBuilder builder) where T : IExceptionHandler
        {
            var exceptionHandler = builder.ApplicationServices.GetService<T>();
            if (exceptionHandler != null)
            {
                builder.AddExceptionHandler(exceptionHandler);
            }

            return builder;
        }
    }
}
