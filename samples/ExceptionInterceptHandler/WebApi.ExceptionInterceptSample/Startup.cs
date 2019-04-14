// ASP.NET.Plus
// Copyright (c) 2019 AspNet.Plus.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

//using AspNet.Plus.Infrastructure.Builder;
//using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.Builder;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.ExceptionInterceptSample.ExceptionIntercepts;
using WebApi.ExceptionInterceptSample.ExceptionIntercepts.ExceptionLoggers;

namespace WebApi.ExceptionInterceptSample
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExceptionInterceptManager();
            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configure(IApplicationBuilder app)
        {
            // *** samples 
            // the order of addition determines the sequence of how each Exception Intercept Handler gets called. 
            app.UseExceptionInterceptManager(new ExceptionInterceptOptions());
            app.AddExceptionInterceptHandler(new ExceptionInitializer(new ExceptionCategorizer()));
            app.AddExceptionInterceptHandler(new ExceptionJIRALogger());
            app.AddExceptionInterceptHandler(new ExceptionDbLogger());
            app.AddExceptionInterceptHandler(new ExceptionFinalizer());

            // OR if intercepts are defined in the IoC
            //app.AddExceptionInterceptHandler<ExceptionInitializer>();
            //app.AddExceptionInterceptHandler<ExceptionDbLogger>();
            //app.AddExceptionInterceptHandler<ExceptionJIRALogger>();
            //app.AddExceptionInterceptHandler(typeof(ExceptionFinalizer));

            // needed for WebApi Controllers. 
            // Note: make sure this line is added after the exception intercept manager has been added.

            app.UseMvc();
        }
    }
}
