using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using BaseAPI.Middlewares;
using System.IO;
using Microsoft.Extensions.Options;
using System.Net.Http;
using BaseAPI.Middlewares.Extension;

namespace BaseAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //logging 

            Enum.TryParse(Configuration.GetValue<string>("Logging:LogLevel:Default"), out LogEventLevel logLevel);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(Path.Combine(Environment.CurrentDirectory, "log-{Date}.txt"), logLevel,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            services.AddHttpClient();

            //var id = Guid.NewGuid().ToString();

            //////Configure Http client
            //services.AddHttpClient(Options.DefaultName, c =>
            //{
            //    c.DefaultRequestHeaders.Add("CorrelationId", id);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseSecurityHeadersMiddleware(
            //   new SecurityHeadersBuilder()
            //       .AddDefaultSecurePolicy()
            //       .AddCustomHeader("X-My-Custom-Header", Guid.NewGuid().ToString())
            //       );

            app.UseLoggerMiddleware(
                new CustomHeaderBuilder()
                .AddDefaultSecurePolicy()
                .AddCustomHeader("ObjId", Guid.NewGuid().ToString()));

            //app.UseCustomMiddleware();

            //app.UseMiddleware<SerilogMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
