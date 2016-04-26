// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using AspNet.Plus.Infrastructure.Builder;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Mvc.ExceptionInterceptSample.ExceptionIntercepts;
using Mvc.ExceptionInterceptSample.ExceptionIntercepts.ExceptionLoggers;
using System;
using System.Threading.Tasks;

/// <summary>
/// This Example demonstrates how you can still show MVC's developer exception pages, if any.
/// In essence, if the Exception Intercept Manager is requested to re-throw the original exception, the middleware will ensure
/// that the original exception is bubbled up to the next Middleware in the pipeline.
/// </summary>
namespace Mvc.ExceptionInterceptSample
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddExceptionInterceptManager();

            // required if adding exception intercept handlers using IoC
            //services.AddSingleton<ExceptionCategorizer>();
            //services.AddSingleton<ExceptionInitializer>();
            //services.AddSingleton<ExceptionFinalizer>();
            //services.AddSingleton(typeof(ExceptionDbLogger));
            //services.AddSingleton(typeof(ExceptionJIRALogger));
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configure(IApplicationBuilder app)  
        {
            // make sure that this line is injected first before adding the Exception Intercept Manager
            app.UseDeveloperExceptionPage();

            // *** samples 
            // the order of addition determines the sequence of how each Exception Intercept Handler gets called. 
            // Setting RethrowException = true, forces the original exception to be bubbled up to the next Middleware in the pipeline.
            app.UseExceptionInterceptManager(new ExceptionInterceptOptions() { RethrowException = true }); 
            app.AddExceptionInterceptHandler(new ExceptionInitializer(new ExceptionCategorizer()));
            app.AddExceptionInterceptHandler(new ExceptionJIRALogger());
            app.AddExceptionInterceptHandler(new ExceptionDbLogger());

            // OR if intercepts are defined in the IoC
            //app.AddExceptionInterceptHandler<ExceptionInitializer>();
            //app.AddExceptionInterceptHandler<ExceptionDbLogger>();
            //app.AddExceptionInterceptHandler<ExceptionJIRALogger>();

            // force the exception
            // The broken section of our application.
            app.Map("/throw", throwApp =>
            {
                throwApp.Run(context => { throw new Exception("Oh my goodness...WTF!"); });
            });

            // The home page.
            app.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>Welcome to the sample<br><br>\r\n");
                await context.Response.WriteAsync("Click here to throw an exception: <a href=\"/throw\">throw</a>\r\n");
                await context.Response.WriteAsync("</body></html>\r\n");
            });
        }
        
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
