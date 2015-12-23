using AspNet.Plus.Infrastructure.Builder;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Mvc.ExceptionInterceptSample.ExceptionIntercepts;
using Mvc.ExceptionInterceptSample.ExceptionIntercepts.ExceptionLoggers;
using System;

/// <summary>
/// This Example demonstrates how you can still show MVC's developer exception pages, if any.
/// In essence, if the Exception Intercept Manager is requested to re-throw the original exception, the middleware will ensure
/// that the original exception is bubbled up to the next Middleware in the pipeline.
/// </summary>
namespace Mvc.ExceptionInterceptSample
{
    public class Startup
    {
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
            app.Run(context => { throw new Exception("Application Exception"); });
        }
        
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
