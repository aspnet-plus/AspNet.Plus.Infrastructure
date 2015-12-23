using AspNet.Plus.Infrastructure.Builder;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler;
using AspNet.Plus.Infrastructure.ExceptionInterceptHandler.Interfaces;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.TestHost;
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
            using (var server = TestServer.Create(
                app =>
                {
                    // act
                    var service = app.ApplicationServices.GetService<ExceptionInterceptManager>();

                    // assert
                    Assert.IsAssignableFrom<ExceptionInterceptManager>(service);
                },
                services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                }))
            {
            }
        }

        [Fact]
        public async void UseExceptionInterceptManager_AddingHandlerThrowingException_ExpectHandlerInvokedAndExceptedException()
        {
            var handler = Substitute.For<IExceptionInterceptHandler>();
            var thrownException = new InvalidOperationException("SomeException");
            var expectedException = (Exception)null;

            using (var server = TestServer.Create(
                app =>
                {
                    // arrange
                    app.UseExceptionInterceptManager();
                    app.AddExceptionInterceptHandler(handler);

                    // assign exception received to expectedException
                    handler.When(x => x.HandleAsync(Arg.Any<IExceptionInterceptContext>())).Do(x => expectedException = x.Arg<IExceptionInterceptContext>().Exception);

                    // act
                    app.Run(httpContext =>
                    {
                        // throw an application exception.
                        throw thrownException;
                    });
                },
                services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                }))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync(string.Empty);
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

            using (var server = TestServer.Create(
                app =>
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
                },
                services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                    services.AddSingleton<FakeExceptionInterceptHandler>();
                }))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync(string.Empty);
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

            using (var server = TestServer.Create(
                app =>
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
                },
                services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                    services.AddSingleton<IExceptionInterceptHandler, FakeExceptionInterceptHandler>();
                }))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync(string.Empty);
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

            using (var server = TestServer.Create(
                app =>
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
                },
                services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                }))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync(string.Empty);
                }
            }

            // assert
            Assert.Null(receivedException);
        }

        [Fact]
        public async void UseExceptionInterceptManager_OptionRethrowExceptionTrue_FakeMiddlewareWillReceiveBubbledException()
        {
            var receivedException = (Exception)null;
            var thrownException = new InvalidOperationException("SomeException");

            using (var server = TestServer.Create(
                app =>
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
                },
                services =>
                {
                    // arrange
                    services.AddExceptionInterceptManager();
                }))
            {
                using (var client = server.CreateClient())
                {
                    await client.GetAsync(string.Empty);
                }
            }

            // assert
            Assert.Same(thrownException, receivedException);
        }
    }
}
