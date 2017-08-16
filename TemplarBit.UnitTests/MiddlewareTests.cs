﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace TemplarBit.UnitTests
{
    public class MiddlewareTests
    {
        private ManualResetEvent _event = new ManualResetEvent(false);
        private TestServer _server;
        private HttpClient _client;
        public MiddlewareTests()
        {
            _event.Set();
        }
        [Fact]
        public async Task ResponseTest()
        {
            try
            {
                _event.WaitOne();
                _event.Reset();
                TestLogger.Logs.Clear();
                Startup.TemplarBitApiToken = "b46b86317e73d423ba8a802f33837b46ce0ba64a0bca55dcba5bcf4bc5cd4a01";
                Startup.TemplarBitPropertyId = "cccb512f-0aa0-4931-89f4-76cda8602a56";
                Startup.TemplarBitApiUrl = "https://api.templarbit.com/v1";
                _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                _client = _server.CreateClient();
                var response = await _client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                System.Threading.Thread.Sleep(1000);
                var responseString = await response.Content.ReadAsStringAsync();

                if (TestLogger.Logs.Count != 0)
                {
                    Assert.True(false);
                }
                Assert.Equal("Hello World!", responseString);
                Assert.True(true);
                _event.Set();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public async Task return_500ResponseTest()
        {
            try
            {
                _event.WaitOne();
                _event.Reset();
                TestLogger.Logs.Clear();
                Startup.TemplarBitApiToken = "return_500";
                Startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                Startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                _client = _server.CreateClient();
                System.Threading.Thread.Sleep(1000);
                var response = await _client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                if (TestLogger.Logs.Count == 0)
                {
                    Assert.True(false);
                }
                Assert.Equal("\nTemplarBitMiddlewareError: Fetch failed, returned status InternalServerError\n", TestLogger.Logs[TestLogger.Logs.Count - 1]);
                Assert.True(true);
                _event.Set();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public async Task return_validResponseTest()
        {
            try
            {
                _event.WaitOne();
                _event.Reset();
                TestLogger.Logs.Clear();
                Startup.TemplarBitApiToken = "return_valid";
                Startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                Startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                _client = _server.CreateClient();
                System.Threading.Thread.Sleep(1000);
                var response = await _client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                if (TestLogger.Logs.Count != 0)
                {
                    Assert.True(false, TestLogger.Logs[0]);
                }
                Assert.True(true);
                _event.Set();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public async Task return_invalidResponseTest()
        {
            try
            {
                _event.WaitOne();
                _event.Reset();
                TestLogger.Logs.Clear();
                Startup.TemplarBitApiToken = "return_invalid";
                Startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                Startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                _client = _server.CreateClient();
                System.Threading.Thread.Sleep(1000);
                var response = await _client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                if (TestLogger.Logs.Count == 0)
                {
                    Assert.True(false);
                }
                Assert.Equal("\nTemplarBitMiddlewareError: Fetch successful, but Content-Security-Policy was empty.\n", TestLogger.Logs[TestLogger.Logs.Count - 1]);
                Assert.True(true);
                _event.Set();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public async Task return_errorResponseTest()
        {
            try
            {
                _event.WaitOne();
                _event.Reset();
                TestLogger.Logs.Clear();
                Startup.TemplarBitApiToken = "return_error";
                Startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                Startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                _client = _server.CreateClient();
                System.Threading.Thread.Sleep(1000);
                var response = await _client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                if (TestLogger.Logs.Count == 0)
                {
                    Assert.True(false);
                }
                Assert.StartsWith("\nTemplarBitMiddlewareError: Fetch failed: ", TestLogger.Logs[TestLogger.Logs.Count - 1]);
                _event.Set();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
        [Fact]
        public async Task return_401ResponseTest()
        {
            try
            {
                _event.WaitOne();
                _event.Reset();
                TestLogger.Logs.Clear();
                Startup.TemplarBitApiToken = "return_401";
                Startup.TemplarBitPropertyId = "571f4f43-ad7a-415d-894b-1a1f234899db";
                Startup.TemplarBitApiUrl = "https://api.tb-stag-01.net/v1";
                _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                _client = _server.CreateClient();
                System.Threading.Thread.Sleep(1000);
                var response = await _client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                if (TestLogger.Logs.Count == 0)
                {
                    Assert.True(false);
                }
                Assert.Equal("\nTemplarBitMiddlewareError: invalid templarbit_api_token and/or templarbit_property_id\n", TestLogger.Logs[TestLogger.Logs.Count - 1]);
                Assert.True(true);
                _event.Set();
            }
            catch (Exception ex)
            {
                Assert.True(false);
                _event.Set();
            }
        }
    }
}
