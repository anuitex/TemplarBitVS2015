using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TemplarBit.Core;

namespace TemplarBit.UnitTests
{
    public class Startup
    {
        public string TemplarBitApiToken { get; set; }
        public string TemplarBitPropertyId { get; set; }
        public string TemplarBitApiUrl { get; set; } = "";
        public ITemplarBitLogger Logger { get; set; }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseTemplarBit(new TemplarBitMiddlewareModel()
            {
                TemplarBitApiToken = TemplarBitApiToken,
                TemplarBitPropertyId = TemplarBitPropertyId,
                TemplarBitApiUrl = TemplarBitApiUrl
            }, Logger);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
        //public static void Main(string[] args)
        //{
        //    var host = new WebHostBuilder()
        //        .UseKestrel()
        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //        .UseIISIntegration()
        //        .UseStartup<Startup>()
        //        .Build();

        //    host.Run();
        //}
    }
}
