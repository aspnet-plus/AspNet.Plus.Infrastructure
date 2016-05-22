// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System;
using Xunit; 

namespace AspNet.Plus.Infrastructure.ExceptionIntercept.Tests
{
    public class ExceptionInterceptManagerTest
    {
        #region Helpers
        public interface IDependencies
        {
            HttpContext HttpContext { get; }            
        }

        private IDependencies CreateDependencies()
        {
            var dependencies = Substitute.For<IDependencies>();            
            return dependencies;
        }
        #endregion Helpers

        #region InterceptExceptionAsync
        [Fact]
        public async void InterceptExceptionAsync_AddingInterceptHandlers_HandlersWereInvokedAndInSequence()
        {
            // arrange            
            var dependencies = CreateDependencies();
            var exceptionInterceptContext = new ExceptionInterceptContext() { Context = dependencies.HttpContext, Exception = new Exception("SomeException") };
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
            await unit.InterceptAsync(exceptionInterceptContext);

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
            var exceptionInterceptContext = new ExceptionInterceptContext() { Context = dependencies.HttpContext, Exception = new Exception("SomeException") };
            var unit = new ExceptionInterceptManager();
            var handler1 = Substitute.For<IExceptionInterceptHandler>();
            var handler2 = Substitute.For<IExceptionInterceptHandler>();
            var handler3 = Substitute.For<IExceptionInterceptHandler>();

            // force handlers to throw an internal exception
            handler1.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Throw(new Exception("Exception1"));
            handler2.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Throw(new Exception("Exception2"));
            handler3.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Throw(new Exception("Exception3"));

            // add the handlers
            unit.AddExceptionInterceptHandler(handler1);
            unit.AddExceptionInterceptHandler(handler2);
            unit.AddExceptionInterceptHandler(handler3);

            // act/assert
            var aggException = await Assert.ThrowsAnyAsync<AggregateException>(async () =>
            {
                await unit.InterceptAsync(exceptionInterceptContext);
            });

            // assert invocations 
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
