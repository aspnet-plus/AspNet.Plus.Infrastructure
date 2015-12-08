using AspNet.Plus.Infrastructure.Builder;
using ExceptionMangerSample.RealisticSamples;
using ExceptionMangerSample.RealisticSamples.ExceptionLoggers;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ExceptionMangerSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddExceptionManager();

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
            app.UseExceptionManager();
            app.AddExceptionHandler(new ExceptionInitializer(new ExceptionCategorizer()));
            app.AddExceptionHandler(new ExceptionJIRALogger());
            app.AddExceptionHandler(new ExceptionDbLogger());
            app.AddExceptionHandler(new ExceptionFinalizer());

            // OR if intercepts are defined in the IoC
            //app.AddExceptionHandler<ExceptionInitializer>();
            //app.AddExceptionHandler<ExceptionDbLogger>();
            //app.AddExceptionHandler<ExceptionJIRALogger>();
            //app.AddExceptionHandler(typeof(ExceptionFinalizer));

            app.UseMvc();
        }
    }
}
