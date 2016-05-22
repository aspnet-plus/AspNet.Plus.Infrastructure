// ASP.NET.Plus
// Copyright (c) 2016 ZoneMatrix Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using AspNet.Plus.Infrastructure.Builder;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Plus.Infrastructure.ExceptionIntercept.Tests.Builder
{
    public class ExceptionInterceptManagerExtensionsTest
    {
        #region Fake Intercepts
        class FakeExceptionInterceptHandler : IExceptionInterceptHandler
        {
            public Exception ExceptionReceived { get; private set; }

            public Task HandleAsync(IExceptionInterceptContext exceptionContext)
            {
                ExceptionReceived = exceptionContext.Exception;
                return Task.FromResult(0);
            }
        }
        #endregion Fake Intercepts

        [Fact]
        public void AddExceptionInterceptManager_AddTheExceptionManagerToIoC_ExpectExceptionInterceptManagerObjectFromIoC()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                })
                .Configure(app =>
                {
                    // act
                    var service = app.ApplicationServices.GetService<ExceptionInterceptManager>();

                    // assert
                    Assert.IsAssignableFrom<ExceptionInterceptManager>(service);
                });

            using (var server = new TestServer(builder))
            {
            }
        }

        [Fact]
        public async void UseExceptionInterceptManager_AddingHandlerThrowingException_ExpectHandlerInvokedAndExceptedException()
        {
            var handler = Substitute.For<IExceptionInterceptHandler>();
            var thrownException = new InvalidOperationException("SomeException");
            var expectedException = (Exception)null;

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                })
                .Configure(app =>
                {
                    // arrange
                    app.UseExceptionInterceptManager();
                    app.AddExceptionInterceptHandler(handler);

                    // assign exception received to expectedException variable to assert later
                    handler.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Do(x => expectedException = x.Arg<IExceptionInterceptContext>().Exception);

                    // act
                    app.Run(httpContext =>
                    {
                        // throw an application exception.
                        throw thrownException;
                    });
                });

            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync("http://someurl.com");
                }
            }

            // assert invocation
            await handler.Received(1).HandleAsync(Arg.Any<IExceptionInterceptContext>());

            // assert argument
            Assert.Same(expectedException, thrownException);
        }

        [Fact]
        public async void UseExceptionInterceptManager_AddingHandlerByClassType_ClassTypeWasSuccessfullyAddedAndInvoked()
        {
            var handler = (FakeExceptionInterceptHandler)null;
            var thrownException = new InvalidOperationException("SomeException");

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                    services.AddSingleton<FakeExceptionInterceptHandler>();
                })
                .Configure(app =>
                {
                    // arrange
                    app.UseExceptionInterceptManager();
                    app.AddExceptionInterceptHandler(typeof(FakeExceptionInterceptHandler)); // adding by class type

                    handler = app.ApplicationServices.GetService<FakeExceptionInterceptHandler>();

                    // act
                    app.Run(httpContext =>
                    {
                        throw thrownException;
                    });
                });

            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync("http://someurl.com");
                }
            }
            
            // assert argument
            Assert.Same(handler.ExceptionReceived, thrownException);
        }

        [Fact]
        public async void UseExceptionInterceptManager_AddingHandlerByInterfaceType_InterfaceTypeWasSuccessfullyAddedAndInvoked()
        {
            var handler = (FakeExceptionInterceptHandler)null;
            var thrownException = new InvalidOperationException("SomeException");

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                    services.AddSingleton<IExceptionInterceptHandler, FakeExceptionInterceptHandler>();

                })                
                .Configure(app =>
                {
                    // arrange
                    app.UseExceptionInterceptManager();
                    app.AddExceptionInterceptHandler<IExceptionInterceptHandler>(); // adding by interface

                    handler = (FakeExceptionInterceptHandler)app.ApplicationServices.GetService<IExceptionInterceptHandler>();

                    // act
                    app.Run(httpContext =>
                    {
                        throw thrownException;
                    });
                });
            
            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync("http://someurl.com");
                }
            }

            // assert argument
            Assert.Same(handler.ExceptionReceived, thrownException);
        }

        [Fact]
        public async void UseExceptionInterceptManager_OptionRethrowExceptionFalse_FakeMiddlewareWillNotReceiveBubbledException()
        {
            var receivedException = (Exception)null;
            var thrownException = new InvalidOperationException("SomeException");
            var exceptionBuddled = false;

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();                    
                })
                .Configure(app =>
                {
                    // inject a fake middleware in the pipeline.
                    app.Use(next => async context =>
                    {
                        try
                        {
                            await next.Invoke(context);
                        }
                        catch (Exception ex)
                        {
                            receivedException = ex; // this exception should not be received.
                            exceptionBuddled = true;
                        }
                    });

                    // arrange
                    var option = new ExceptionInterceptOptions() { RethrowException = false }; // don't bubble exception
                    app.UseExceptionInterceptManager(option);

                    // act
                    app.Run(httpContext =>
                    {
                        throw thrownException;
                    });
                });

            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync("http://someurl.com");
                }
            }

            // assert
            Assert.Null(receivedException);
            Assert.False(exceptionBuddled, "The exception should not have been bubbled.");
        }

        [Fact]
        public async void UseExceptionInterceptManager_OptionRethrowExceptionTrue_FakeMiddlewareWillReceiveBubbledException()
        {
            var receivedException = (Exception)null;
            var thrownException = new InvalidOperationException("SomeException");
            var exceptionBuddled = false;

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                })
                .Configure(app =>
                {
                    // inject a fake middleware in the pipeline.
                    app.Use(next => async context =>
                    {
                        try
                        {
                            await next.Invoke(context);
                        }
                        catch (Exception ex)
                        {
                            receivedException = ex; // this exception should be received.
                            exceptionBuddled = true;
                        }
                    });

                    // arrange
                    var option = new ExceptionInterceptOptions() { RethrowException = true }; // bubble exception
                    app.UseExceptionInterceptManager(option);

                    // act
                    app.Run(httpContext =>
                    {
                        throw thrownException;
                    });
                });

            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync("http://someurl.com");
                }
            }

            // assert
            Assert.Same(thrownException, receivedException);
            Assert.True(exceptionBuddled, "The exception should have been bubbled.");
        }
    }
}
