using System;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using NSubstitute;
using Xunit;


//    "xunit": "2.1.0",
//    "xunit.runner.dnx": "2.1.0-rc1-build204",

//"test": "xunit.runner.dnx",


namespace AspNet.Plus.Infrastructure.ExceptionIntercept.Tests
{
    public class ExceptionInterceptManagerTest
    {
        #region Helpers
        public interface IDependencies
        {
            HttpContext HttpContext { get; }
            IExceptionHandlerFeature ExceptionFeature { get; }
        }
        
        private IDependencies CreateDependencies()
        {
            var dependencies = Substitute.For<IDependencies>();
            dependencies.HttpContext.Features[Arg.Is<Type>(typeof(IExceptionHandlerFeature))].Returns(dependencies.ExceptionFeature);
                        
            return dependencies;
        }
        #endregion Helpers

        #region InterceptExceptionAsync
        [Fact]
        public async void InterceptExceptionAsync_AddingInterceptHandlers_HandlersWereInvokedAndInSequence()
        {
            // arrange            
            var dependencies = CreateDependencies();
            var unit = new ExceptionInterceptManager();
            var handler1 = Substitute.For<IExceptionInterceptHandler>();
            var handler2 = Substitute.For<IExceptionInterceptHandler>();
            var handler3 = Substitute.For<IExceptionInterceptHandler>();

            var counter = 1;
            var atHandler1Call = -1;
            var atHandler2Call = -1;
            var atHandler3Call = -1;

            handler1.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Do(x => atHandler1Call = counter++);
            handler2.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Do(x => atHandler2Call = counter++);
            handler3.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Do(x => atHandler3Call = counter++);
            
            // add the handlers
            unit.AddExceptionInterceptHandler(handler1);
            unit.AddExceptionInterceptHandler(handler2);
            unit.AddExceptionInterceptHandler(handler3);

            // act
            await unit.InterceptExceptionAsync(dependencies.HttpContext);

            // assert invokes
            await handler1.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());
            await handler2.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());
            await handler3.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());

            // assert sequence
            Assert.Equal(atHandler1Call, 1);
            Assert.Equal(atHandler2Call, 2);
            Assert.Equal(atHandler3Call, 3);
        }

        [Fact]
        public async void InterceptExceptionAsync_InterceptHandlersThrowingExceptions_AggregateExceptionAndInnerExceptions()
        {
            // arrange            
            var dependencies = CreateDependencies();
            var unit = new ExceptionInterceptManager();
            var handler1 = Substitute.For<IExceptionInterceptHandler>();
            var handler2 = Substitute.For<IExceptionInterceptHandler>();
            var handler3 = Substitute.For<IExceptionInterceptHandler>();
            
            handler1.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Throw(new Exception("Exception1"));
            handler2.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Throw(new Exception("Exception2"));
            handler3.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Throw(new Exception("Exception3"));

            // add the handlers
            unit.AddExceptionInterceptHandler(handler1);
            unit.AddExceptionInterceptHandler(handler2);
            unit.AddExceptionInterceptHandler(handler3);

            // act
            var aggException = await Assert.ThrowsAnyAsync<AggregateException>(async () =>
            {
                await unit.InterceptExceptionAsync(dependencies.HttpContext);
            });

            // assert invokes
            await handler1.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());
            await handler2.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());
            await handler3.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());

            Assert.Equal(aggException.InnerExceptions[0].Message, "Exception1");
            Assert.Equal(aggException.InnerExceptions[1].Message, "Exception2");
            Assert.Equal(aggException.InnerExceptions[2].Message, "Exception3");
        }
        #endregion InterceptExceptionAsync
    }
}
