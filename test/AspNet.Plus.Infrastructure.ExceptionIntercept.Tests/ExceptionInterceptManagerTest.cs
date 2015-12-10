using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Plus.Infrastructure.ExceptionIntercept.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1
    {
        //private IMock<HttpContext> PrepareHttpContextMock()
        //{
        //    var httpContext = new Mock<HttpContext>();

        //    httpContext.Setup(x => x.Features).Returns(new Mock<IFeatureCollection>().Object);

        //    return httpContext;
        //}

        [Fact]
        public void InterceptExceptionAsync_AddingIntercetHandlers_HandlersWereInvokedInSequence()
        {
            //    // arrange            
            //    var unit = new ExceptionInterceptManager();
            //    var handler1 = new Mock<IExceptionInterceptHandler>();
            //    var handler2 = new Mock<IExceptionInterceptHandler>();
            //    var handler3 = new Mock<IExceptionInterceptHandler>();
            //    var httpContext = new Mock<HttpContext>();



            //    //var xx = httpContext.Object.Features;



            //    // var xxx = new Mock<IExceptionInterceptContext>();
            //    // new Mock<IExceptionHandlerFeature>();


        }
    }
}
