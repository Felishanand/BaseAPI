using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAPI.Middlewares
{
    public class SerilogMiddleware
    {
        private const string RequestMessageTemplate = "{TrackingId} Request: HTTP {RequestMethod} {RequestPath}";

        private const string ResponseMessageTemplate =

            "{TrackingId} Response: HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();

        private readonly RequestDelegate _next;

        public SerilogMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var correlationId = Guid.NewGuid().ToString();

            var sw = Stopwatch.StartNew();

            try

            {
                // access_token, condition checked so that SignalR request won't be logged

                if (!(httpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) && httpContext.Request.QueryString.Value.Contains("access_token=")))
                {
                    Log.Write(LogEventLevel.Information, RequestMessageTemplate, correlationId, httpContext.Request.Method, httpContext.Request.Path);
                }

                // adding tracking id in each request

                httpContext.Request.Headers.Add("CorrelationId", correlationId);

                var logger = httpContext.RequestServices.GetRequiredService<ILogger<SerilogMiddleware>>();

                if (logger != null)

                {
                    using (logger.BeginScope("{@CorrelationId}", correlationId))

                    {
                        // send for next pipeline

                        await _next(httpContext);
                    }
                }
                else

                {
                    // send for next pipeline

                    await _next(httpContext);
                }

                sw.Stop();

                var statusCode = httpContext.Response?.StatusCode;

                var level = statusCode > 399 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;

                log.Write(level, ResponseMessageTemplate, correlationId, httpContext.Request.Method, httpContext.Request.Path, statusCode, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex) when (LogException(httpContext, sw, ex, correlationId)) { }
        }

        private static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex, string correlationId)
        {
            sw.Stop();

            LogForErrorContext(httpContext)
                .Error(ex, ResponseMessageTemplate, correlationId, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);

            return false;
        }

        private static Serilog.ILogger LogForErrorContext(HttpContext httpContext)

        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)

                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }
    }
}