using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAPI.Middlewares.Extension
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return app.UseMiddleware<SecurityHeadersMiddleware>(builder.Build());
        }
    }
}
