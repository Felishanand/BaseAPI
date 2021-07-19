using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CustomHeadersPolicy _policy;

        /// <summary>
        /// Instantiates a new <see cref="LoggerMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="policy">An instance of the <see cref="SecurityHeadersPolicy"/> which can be applied.</param>
        public LoggerMiddleware(RequestDelegate next, CustomHeadersPolicy policy)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _next = next;
            _policy = policy;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var request = context.Request;

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }            


            var headers = request.Headers;

            foreach (var headerValuePair in _policy.SetHeaders)
            {
                headers[headerValuePair.Key] = headerValuePair.Value;                
            }

            foreach (var header in _policy.RemoveHeaders)
            {
                headers.Remove(header);
            }

            //var response = context.Response;

            //if (response == null)
            //{
            //    throw new ArgumentNullException(nameof(response));
            //}

            //var headers = response.Headers;

            //foreach (var headerValuePair in _policy.SetHeaders)
            //{
            //    headers[headerValuePair.Key] = headerValuePair.Value;
            //}

            //foreach (var header in _policy.RemoveHeaders)
            //{
            //    headers.Remove(header);
            //}

            await _next(context);
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder app, CustomHeaderBuilder builder)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return app.UseMiddleware<LoggerMiddleware>(builder.Build());
        }       
    }


}
