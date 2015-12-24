// ASP.NET.Plus
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNet.Builder;

namespace AspNet.Plus.Infrastructure.ExceptionInterceptHandler
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionInterceptMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly ExceptionInterceptOptions _options;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ExceptionInterceptManager _exceptionInterceptManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInterceptMiddleware"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="next">The next.</param>
        /// <param name="options">The options.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="exceptionInterceptManager">The exception intercept manager.</param>
        public ExceptionInterceptMiddleware(
            ILoggerFactory loggerFactory,
            RequestDelegate next,
            ExceptionInterceptOptions options,
            DiagnosticSource diagnosticSource,            
            ExceptionInterceptManager exceptionInterceptManager)
        {
            _next = next;
            _options = options;
            _diagnosticSource = diagnosticSource;
            _exceptionInterceptManager = exceptionInterceptManager;
            _logger = loggerFactory.CreateLogger<ExceptionInterceptMiddleware>();            
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                try
                {               
                    if (!_options.RethrowException)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = 500;
                        context.Response.OnStarting(state =>
                        {
                            var response = (HttpResponse)state;
                            response.Headers[HeaderNames.CacheControl] = "no-cache";
                            response.Headers[HeaderNames.Pragma] = "no-cache";
                            response.Headers[HeaderNames.Expires] = "-1";
                            response.Headers.Remove(HeaderNames.ETag);
                            return Task.FromResult(0);
                        }, 
                        context.Response);
                    }

                    var exceptionHandlerFeature = new ExceptionHandlerFeature()
                    {
                        Error = ex,
                    };

                    context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                    
                    var exceptionContext = new ExceptionInterceptContext()
                    {
                        Context = context,
                        Exception = ex
                    };

                    await _exceptionInterceptManager.InterceptAsync(exceptionContext);
                    
                    if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Diagnostics.HandledException"))
                    {
                        _diagnosticSource.Write("Microsoft.AspNet.Diagnostics.HandledException", new { httpContext = context, exception = ex });
                    }

                    if (!_options.RethrowException)
                    {
                        return;
                    }
                }
                catch (Exception ex2)
                {
                    // suppress secondary exceptions, re-throw the original.
                    _logger.LogError("An exception was thrown attempting to execute the exception intercept handler.", ex2);
                }

                throw; // re-throw original if requested.
            }
        }
    }
}