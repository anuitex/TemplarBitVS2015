using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace TemplarBit.UnitTests
{
    public class MiddlewareTests
    {
        private ManualResetEvent _event = new ManualResetEvent(false);
        private readonly int _timeOut = 0;
        public MiddlewareTests()
        {
            _event.Set();

        }
        [Fact]
        public void ResponseTest()
        {
            try
            {
                var thread = new Thread(async () =>
                {
                    _event.WaitOne();
                    _event.Reset();
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarBitApiToken = "b46b86317e73d423ba8a802f33837b46ce0ba64a0bca55dcba5bcf4bc5cd4a01";
                    startup.TemplarBitPropertyId = "cccb512f-0aa0-4931-89f4-76cda8602a56";
                    startup.TemplarBitApiUrl = "https://api.templarbit.com/v1";
                    var builder = new WebHostBuilder()
                             .ConfigureServices(services =>
                             {
                                 services.AddSingleton<Startup>(startup);
                             });

                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();

                    System.Threading.Thread.Sleep(1000);
                    var responseString = await response.Content.ReadAsStringAsync();

                    // Assert
                    if (logger.Logs.Count != 0)
                    {
                        Assert.True(false);
                    }
                    Assert.True(true);
                    _event.Set();
                });
                thread.Start();

            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public void return_500ResponseTest()
        {
            try
            {
                var thread = new Thread(async () =>
                {
                    _event.WaitOne();
                    _event.Reset();
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarBitApiToken = "return_500";
                    startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                             .ConfigureServices(services =>
                             {
                                 services.AddSingleton<Startup>(startup);
                             });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(2000);
                    // Act
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();


                    // Assert
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.Equal("\nTemplarBitMiddlewareError: Fetch failed, returned status InternalServerError\n", logger.Logs[logger.Logs.Count - 1]);
                    Assert.True(true);
                    _event.Set();
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public void return_validResponseTest()
        {
            try
            {
                var thread = new Thread(async () =>
                {
                    _event.WaitOne();
                    _event.Reset();
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarBitApiToken = "return_valid";
                    startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";

                    var builder = new WebHostBuilder()
                              .ConfigureServices(services =>
                              {
                                  services.AddSingleton<Startup>(startup);
                              });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(3000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    if (logger.Logs.Count != 0)
                    {
                        Assert.True(false, logger.Logs[0]);
                    }
                    Assert.True(true);
                    _event.Set();
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public void return_invalidResponseTest()
        {
            try
            {
                var thread = new Thread(async () =>
                {
                    _event.WaitOne();
                    _event.Reset();
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarBitApiToken = "return_invalid";
                    startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                              .ConfigureServices(services =>
                              {
                                  services.AddSingleton<Startup>(startup);
                              });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(4000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    Thread.Sleep(_timeOut);
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.Equal("\nTemplarBitMiddlewareError: Fetch successful, but Content-Security-Policy was empty.\n", logger.Logs[logger.Logs.Count - 1]);
                    Assert.True(true);
                    _event.Set();
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public void return_errorResponseTest()
        {
            try
            {
                var thread = new Thread(async () =>
                {
                    _event.WaitOne();
                    _event.Reset();
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarBitApiToken = "return_error";
                    startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                             .ConfigureServices(services =>
                             {
                                 services.AddSingleton<Startup>(startup);
                             });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(1000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.True(logger.Logs[logger.Logs.Count - 1].StartsWith("\nTemplarBitMiddlewareError: Fetch failed: "));
                    _event.Set();
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public void return_401ResponseTest()
        {
            try
            {
                var thread = new Thread(async () =>
                {
                    _event.WaitOne();
                    _event.Reset();
                    var logger = new TestLogger();
                    var startup = new Startup();
                    startup.Logger = logger;
                    startup.TemplarBitApiToken = "return_401";
                    startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                    startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                    var builder = new WebHostBuilder()
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton<Startup>(startup);
                            });
                    var server = new TestServer(builder.UseStartup<Startup>());
                    var client = server.CreateClient();
                    System.Threading.Thread.Sleep(6000);
                    var response = await client.GetAsync("/");
                    response.EnsureSuccessStatusCode();
                    Thread.Sleep(_timeOut);
                    if (logger.Logs.Count == 0)
                    {
                        Assert.True(false);
                    }
                    Assert.Equal("\nTemplarBitMiddlewareError: invalid templarbit_api_token and/or templarbit_property_id\n", logger.Logs[logger.Logs.Count - 1]);
                    Assert.True(true);
                    _event.Set();
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
    }
}
