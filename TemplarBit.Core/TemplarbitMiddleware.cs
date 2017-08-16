using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net;

namespace TemplarBit.Core
{
    public class TemplarBitMiddleware
    {
        public static bool Digest { get; set; } = true;
        private const int MILLISECONDS_IN_SECOND = 1000;
        private readonly RequestDelegate _next;
        private readonly TemplarBitMiddlewareModel _model;
        private readonly ITemplarBitLogger _logger;
        private string contentSecurityPolicy = "";
        private string contentSecurityPolicyReportOnly = "";
        public TemplarBitMiddleware(RequestDelegate next, TemplarBitMiddlewareModel model, ITemplarBitLogger logger)
        {
            _next = next;
            _model = model;
            _logger = logger;

            Initizalization();
            if (!String.IsNullOrEmpty(_model.TemplarBitPropertyId) && !String.IsNullOrEmpty(_model.TemplarBitApiToken))
            {
                Task.Run(() => StartLoop());
            }
        }

        private async Task StartLoop()
        {
            while (Digest)
            {
                try
                {
                    var data = new
                    {
                        property_id = _model.TemplarBitPropertyId,
                        token = _model.TemplarBitApiToken
                    };
                    var jsonData = JsonConvert.SerializeObject(data);
                    var result = await HttpHelper.DataPost(_model.TemplarBitApiUrl + "/csp", jsonData);
                    if (result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        _logger.Log("invalid templarbit_api_token and/or templarbit_property_id");

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarBitFetchInterval));
                        continue;
                    }
                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.Log("Fetch failed, returned status " + result.StatusCode.ToString());

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarBitFetchInterval));
                        continue;
                    }
                    var response = JsonConvert.DeserializeObject<ResponseFromTemplarBit>(result.Response);
                    if (!String.IsNullOrEmpty(response.error))
                    {
                        _logger.Log("Fetch failed: " + response.error);

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarBitFetchInterval));
                        continue;
                    }
                    if (String.IsNullOrEmpty(response.csp) && String.IsNullOrEmpty(response.csp_report_only))
                    {
                        _logger.Log("Fetch successful, but Content-Security-Policy was empty.");

                        System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarBitFetchInterval));
                        continue;
                    }

                    System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarBitFetchInterval));
                    contentSecurityPolicy = response.csp;
                    contentSecurityPolicyReportOnly = response.csp_report_only;
                }
                catch (Exception e)
                {
                    _logger.Log("Fetch failed with error " + e.Message);

                    System.Threading.Thread.Sleep((int)(MILLISECONDS_IN_SECOND * _model.TemplarBitFetchInterval));
                    continue;
                }
            }
        }

        public Task Invoke(HttpContext context)
        {
            try
            {
                if (!String.IsNullOrEmpty(_model.TemplarBitPropertyId) && !String.IsNullOrEmpty(_model.TemplarBitApiToken))
                {
                    Task.Run(() => Run(context));
                }
                // Call the next delegate/middleware in the pipeline
                return this._next(context);
            }
            catch (Exception)
            {
                return this._next(context);
            }

        }

        private void Run(HttpContext context)
        {
            if (!string.IsNullOrEmpty(contentSecurityPolicy))
            {
                try
                {
                    var headers = context.Response.Headers;
                    headers["Content-Security-Policy"] = contentSecurityPolicy;
                }
                catch (Exception e)
                {
                    _logger.Log("Failed to set Content-Security-Policy response header: " + e.Message);
                }
            }
            if (!string.IsNullOrEmpty(contentSecurityPolicyReportOnly))
            {
                try
                {
                    var headers = context.Response.Headers;
                    headers["Content-Security-Policy-Report-Only"] = contentSecurityPolicyReportOnly;
                }
                catch (Exception e)
                {
                    _logger.Log("Failed to set Content-Security-Policy-Report-Only response header: " + e.Message);
                }
            }
        }

        private void Initizalization()
        {
            if (String.IsNullOrEmpty(_model.TemplarBitPropertyId))
            {
                _logger.Log("templarbit_property_id not set");
            }
            if (String.IsNullOrEmpty(_model.TemplarBitApiToken))
            {
                _logger.Log("templarbit_api_token not set");
            }
            if (_model.TemplarBitFetchInterval == 0)
            {
                _model.TemplarBitFetchInterval = 5;
            }
            if (String.IsNullOrEmpty(_model.TemplarBitApiUrl))
            {
                _model.TemplarBitApiUrl = "https://api.templarbit.com/v1";
            }
        }
    }
}
