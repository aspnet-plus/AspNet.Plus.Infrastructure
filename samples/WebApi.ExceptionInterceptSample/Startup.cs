using AspNet.Plus.Infrastructure.Builder;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebApi.ExceptionInterceptSample.ExceptionIntercepts;
using WebApi.ExceptionInterceptSample.ExceptionIntercepts.ExceptionLoggers;

/// <summary>
/// This Example demonstrates how intercept unhandled exceptions and delegate them to intercept handlers.
/// </summary>
namespace WebApi.ExceptionInterceptSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddExceptionInterceptManager();
            services.AddMvc();

            // required if adding exception intercept handlers using IoC
            //services.AddSingleton<ExceptionCategorizer>();
            //services.AddSingleton<ExceptionInitializer>();
            //services.AddSingleton<ExceptionFinalizer>();
            //services.AddSingleton(typeof(ExceptionDbLogger));
            //services.AddSingleton(typeof(ExceptionJIRALogger));
        }

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
        
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
