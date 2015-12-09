using AspNet.Plus.Infrastructure.Builder;
using ExceptionMangerSample.RealisticSamples;
using ExceptionMangerSample.RealisticSamples.ExceptionLoggers;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ExceptionMangerSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
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
            // *** samples 
            // the order of addition determines the sequence of how each Exception Intercept Handler gets called.
            app.UseExceptionInterceptManager();
            app.AddExceptionInterceptHandler(new ExceptionInitializer(new ExceptionCategorizer()));
            app.AddExceptionInterceptHandler(new ExceptionJIRALogger());
            app.AddExceptionInterceptHandler(new ExceptionDbLogger());
            app.AddExceptionInterceptHandler(new ExceptionFinalizer());

            // OR if intercepts are defined in the IoC
            //app.AddExceptionInterceptHandler<ExceptionInitializer>();
            //app.AddExceptionInterceptHandler<ExceptionDbLogger>();
            //app.AddExceptionInterceptHandler<ExceptionJIRALogger>();
            //app.AddExceptionInterceptHandler(typeof(ExceptionFinalizer));

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
